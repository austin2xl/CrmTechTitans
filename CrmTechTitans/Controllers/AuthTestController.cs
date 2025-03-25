using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace CrmTechTitans.Controllers
{
    [AllowAnonymous]
    public class AuthTestController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<AuthTestController> _logger;

        public AuthTestController(
            IConfiguration configuration,
            IWebHostEnvironment environment,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<AuthTestController> logger)
        {
            _configuration = configuration;
            _environment = environment;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("<h1>Authentication Diagnostic Info</h1>");
            sb.AppendLine("<h2>Environment Information</h2>");
            sb.AppendLine($"<p>Environment: {_environment.EnvironmentName}</p>");
            sb.AppendLine($"<p>Is Production: {_environment.IsProduction()}</p>");
            
            sb.AppendLine("<h2>Connection Strings</h2>");
            sb.AppendLine("<ul>");
            foreach (var conn in _configuration.GetSection("ConnectionStrings").GetChildren())
            {
                // Mask the sensitive parts of connection strings
                var connValue = conn.Value ?? "";
                if (connValue.Contains("Password="))
                {
                    connValue = connValue.Replace(connValue.Split("Password=")[1].Split(';')[0], "******");
                }
                sb.AppendLine($"<li>{conn.Key}: {connValue}</li>");
            }
            sb.AppendLine("</ul>");
            
            sb.AppendLine("<h2>Current User</h2>");
            if (User.Identity?.IsAuthenticated == true)
            {
                sb.AppendLine($"<p>Authenticated: Yes</p>");
                sb.AppendLine($"<p>Username: {User.Identity.Name}</p>");
                sb.AppendLine("<p>Claims:</p>");
                sb.AppendLine("<ul>");
                foreach (var claim in User.Claims)
                {
                    sb.AppendLine($"<li>{claim.Type}: {claim.Value}</li>");
                }
                sb.AppendLine("</ul>");
            }
            else
            {
                sb.AppendLine($"<p>Authenticated: No</p>");
            }
            
            sb.AppendLine("<h2>Users in Database</h2>");
            try
            {
                var users = _userManager.Users.Take(10).ToList();
                if (users.Any())
                {
                    sb.AppendLine("<ul>");
                    foreach (var user in users)
                    {
                        sb.AppendLine($"<li>{user.UserName} / {user.Email}</li>");
                    }
                    sb.AppendLine("</ul>");
                }
                else
                {
                    sb.AppendLine("<p>No users found in database</p>");
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine($"<p>Error retrieving users: {ex.Message}</p>");
            }
            
            return Content(sb.ToString(), "text/html");
        }
        
        [HttpGet]
        public IActionResult TestLogin()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> TestLogin(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return View("TestLogin", "Email and password are required");
            }
            
            try
            {
                var result = await _signInManager.PasswordSignInAsync(email, password, false, false);
                
                if (result.Succeeded)
                {
                    return Content("Login successful! You are now authenticated.", "text/html");
                }
                else
                {
                    return View("TestLogin", $"Login failed: {GetSignInResultMessage(result)}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during test login");
                return View("TestLogin", $"Error: {ex.Message}");
            }
        }
        
        private string GetSignInResultMessage(Microsoft.AspNetCore.Identity.SignInResult result)
        {
            if (result.IsLockedOut) return "Account is locked out";
            if (result.IsNotAllowed) return "Login not allowed";
            if (result.RequiresTwoFactor) return "Two factor authentication required";
            return "Invalid login attempt";
        }
    }
} 