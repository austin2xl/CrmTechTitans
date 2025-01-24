namespace CrmTechTitans.Models.JoinTables
{
    public class MemberIndustry
    {
        public int IndustryID { get; set; }
        public Industry? Industry { get; set; }

        public int MemberID { get; set; }
        public Member? Member { get; set; }
    }
}
