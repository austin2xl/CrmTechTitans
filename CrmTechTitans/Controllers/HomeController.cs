using CrmTechTitans.Data;
using CrmTechTitans.Models;
using CrmTechTitans.Models.Enumerations;
using CrmTechTitans.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CrmTechTitans.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CrmContext _context;

        public HomeController(ILogger<HomeController> logger, CrmContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Get total counts for each membership status
                var goodStandingCount = await _context.Members
                    .CountAsync(m => m.MembershipStatus == MembershipStatus.GoodStanding);
                
                var outstandingCount = await _context.Members
                    .CountAsync(m => m.MembershipStatus == MembershipStatus.OutStanding);
                
                var cancelledCount = await _context.Members
                    .CountAsync(m => m.MembershipStatus == MembershipStatus.Cancelled);
                
                var totalMembers = goodStandingCount + outstandingCount + cancelledCount;

                // Create member summary
                var memberSummary = new MemberCountVM
                {
                    Membership = "Summary",
                    TotalMembers = totalMembers,
                    GoodStanding = goodStandingCount,
                    OutStanding = outstandingCount,
                    Cancelled = cancelledCount
                };

                // Get all members with their membership types
                var members = await _context.Members
                    .Include(m => m.MemberMembershipTypes)
                    .ThenInclude(mmt => mmt.MembershipType)
                    .ToListAsync();

                // Create a dictionary to count members by membership type
                var membershipTypeCounts = new Dictionary<string, int>();
                
                // Count members for each membership type
                foreach (var member in members)
                {
                    foreach (var memberMembershipType in member.MemberMembershipTypes)
                    {
                        var typeName = memberMembershipType.MembershipType.Name;
                        if (!membershipTypeCounts.ContainsKey(typeName))
                        {
                            membershipTypeCounts[typeName] = 0;
                        }
                        membershipTypeCounts[typeName]++;
                    }
                }

                // Create membership type summary
                var membershipSummary = new MembershipTypeCountVM();
                
                // Add all membership types to the dictionary
                foreach (var kvp in membershipTypeCounts)
                {
                    membershipSummary.AllMembershipTypes[kvp.Key] = kvp.Value;
                }
                
                // Map the counts to the view model properties for backward compatibility
                membershipSummary.AssociateCount = membershipTypeCounts.GetValueOrDefault("Associate", 0);
                membershipSummary.ChamberAssociateCount = membershipTypeCounts.GetValueOrDefault("Chamber", 0);
                membershipSummary.NonLocalIndustrialCount = membershipTypeCounts.GetValueOrDefault("Non-Local", 0);
                membershipSummary.GovernmentAssociationCount = membershipTypeCounts.GetValueOrDefault("Government Education", 0);
                membershipSummary.LocalCount = membershipTypeCounts.GetValueOrDefault("Local", 0);
                
                // Calculate "Other" as any types not explicitly mapped above
                membershipSummary.OtherCount = membershipTypeCounts
                    .Where(kvp => 
                        kvp.Key != "Associate" && 
                        kvp.Key != "Chamber" && 
                        kvp.Key != "Non-Local" && 
                        kvp.Key != "Government Education" && 
                        kvp.Key != "Local")
                    .Sum(kvp => kvp.Value);

                // Get opportunity counts
                var qualificationCount = await _context.Opportunities
                    .CountAsync(o => o.Status == Status.Qualification);
                
                var negotiationCount = await _context.Opportunities
                    .CountAsync(o => o.Status == Status.Negotiating);
                
                var closedNewMembersCount = await _context.Opportunities
                    .CountAsync(o => o.Status == Status.ClosedNewMember);
                
                var closedNotInterestedCount = await _context.Opportunities
                    .CountAsync(o => o.Status == Status.ClosedNotInterested);

                var opportunitySummary = new OpportunityCountVM
                {
                    QualificationCount = qualificationCount,
                    NegotiationCount = negotiationCount,
                    ClosedNewMembersCount = closedNewMembersCount,
                    ClosedNotInterestedCount = closedNotInterestedCount
                };

                // Get member growth over time - calculate cumulative count
                var membersByDate = members
                    .OrderBy(m => m.MemberSince.Date)
                    .GroupBy(m => new DateTime(m.MemberSince.Year, m.MemberSince.Month, 1)) // Group by month
                    .Select(g => new { Date = g.Key, NewMembers = g.Count() })
                    .ToList();

                var memberCountOverTime = new List<MemberCountOverTimeVM>();
                int cumulativeCount = 0;
                
                // Generate a complete timeline from the earliest member to today
                if (membersByDate.Any())
                {
                    var startDate = membersByDate.Min(m => m.Date);
                    var endDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    
                    for (var date = startDate; date <= endDate; date = date.AddMonths(1))
                    {
                        // Find new members for this month
                        var newMembersInMonth = membersByDate
                            .Where(m => m.Date == date)
                            .Sum(m => m.NewMembers);
                        
                        cumulativeCount += newMembersInMonth;
                        
                        memberCountOverTime.Add(new MemberCountOverTimeVM
                        {
                            Date = date,
                            Count = cumulativeCount
                        });
                    }
                }

                // Combine all summaries into a single view model
                var dashboardViewModel = new DashboardVM
                {
                    MemberCountSummary = memberSummary,
                    MembershipTypeSummary = membershipSummary,
                    OpportunityCountSummary = opportunitySummary,
                    MemberCountOverTime = memberCountOverTime
                };

                _logger.LogInformation("Dashboard data loaded successfully");
                return View(dashboardViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard data");
                
                // Create empty view model to avoid null reference exceptions
                var emptyViewModel = new DashboardVM
                {
                    MemberCountSummary = new MemberCountVM { Membership = "Summary", TotalMembers = 0, GoodStanding = 0, OutStanding = 0, Cancelled = 0 },
                    MembershipTypeSummary = new MembershipTypeCountVM(),
                    OpportunityCountSummary = new OpportunityCountVM(),
                    MemberCountOverTime = new List<MemberCountOverTimeVM>()
                };
                
                return View(emptyViewModel);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
