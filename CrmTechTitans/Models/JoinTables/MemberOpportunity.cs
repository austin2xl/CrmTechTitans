namespace CrmTechTitans.Models.JoinTables
{
    public class MemberOpportunity
    {

        public int MemberID { get; set; }
        public Member? Member { get; set; }

        public int OpportunityID { get; set; }
        public Opportunity? Opportunity { get; set; }
    }
}
