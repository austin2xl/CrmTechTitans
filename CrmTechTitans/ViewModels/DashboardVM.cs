namespace CrmTechTitans.ViewModels
{
    public class DashboardVM
    {
        public MemberCountVM? MemberCountSummary { get; set; }
        public MembershipTypeCountVM? MembershipTypeSummary { get; set; }
        public OpportunityCountVM? OpportunityCountSummary { get; set; }
        public List<MemberCountOverTimeVM>? MemberCountOverTime { get; set; } // New property
        public Dictionary<string, int>? MembersByIndustry { get; set; }
        public Dictionary<string, int>? MembersByMunicipality { get; set; }
    }

    public class MemberCountOverTimeVM
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }
}
