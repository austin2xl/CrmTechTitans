using CrmTechTitans.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string connectionString;
if (builder.Environment.IsProduction())
{
    // Use Azure SQL Database in production
    connectionString = builder.Configuration.GetConnectionString("AzureConnection")
        ?? throw new InvalidOperationException("Connection string 'AzureConnection' not found.");
}
else
{
    // Use SQLite in development
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}

// Get identity options from appsettings.json
var identityOptions = builder.Configuration.GetSection("IdentityOptions");
var requireConfirmedAccount = identityOptions.GetValue<bool>("SignIn:RequireConfirmedAccount", true);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (builder.Environment.IsProduction())
    {
        options.UseSqlServer(connectionString);
    }
    else
    {
        options.UseSqlite(connectionString);
    }
});

builder.Services.AddDbContext<CrmContext>(options =>
{
    if (builder.Environment.IsProduction())
    {
        options.UseSqlServer(connectionString);
    }
    else
    {
        options.UseSqlite(connectionString);
    }
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = requireConfirmedAccount;

    // Configure password requirements from appsettings.json
    if (identityOptions.GetSection("Password") != null)
    {
        options.Password.RequireDigit = identityOptions.GetValue<bool>("Password:RequireDigit", true);
        options.Password.RequireLowercase = identityOptions.GetValue<bool>("Password:RequireLowercase", true);
        options.Password.RequireUppercase = identityOptions.GetValue<bool>("Password:RequireUppercase", true);
        options.Password.RequireNonAlphanumeric = identityOptions.GetValue<bool>("Password:RequireNonAlphanumeric", true);
        options.Password.RequiredLength = identityOptions.GetValue<int>("Password:RequiredLength", 6);
    }
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Add authorization to require login for all pages by default
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddControllersWithViews();

// Add session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Use session middleware
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// Create database and apply migrations before seeding
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("Starting database initialization...");
        
        // Ensure Identity database exists and has been migrated
        var identityContext = services.GetRequiredService<ApplicationDbContext>();
        logger.LogInformation("Applying Identity database migrations...");
        identityContext.Database.Migrate();
        logger.LogInformation("Identity database migrations completed successfully.");

        // Ensure CRM database exists and has been migrated
        var crmContext = services.GetRequiredService<CrmContext>();
        logger.LogInformation("Applying CRM database migrations...");
        crmContext.Database.Migrate();
        logger.LogInformation("CRM database migrations completed successfully.");

        // Initialize CRM data
        logger.LogInformation("Initializing CRM data...");
        CrmInitializer.Initialize(serviceProvider: services);
        logger.LogInformation("CRM data initialization completed successfully.");

        // Initialize Identity roles and users
        logger.LogInformation("Initializing Identity roles and users...");
        await IdentityInitializer.InitializeAsync(services);
        logger.LogInformation("Identity initialization completed successfully.");

        logger.LogInformation("Database initialization completed successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while initializing the database.");
        Console.WriteLine($"Error during database initialization: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
        
        if (ex.InnerException != null)
        {
            logger.LogError(ex.InnerException, "Inner exception details");
            Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
        }
    }
}

app.Run();
