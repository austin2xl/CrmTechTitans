namespace CrmTechTitans.ViewModels
{
    public class MembershipTypeCountVM
    {
        public MembershipTypeCountVM()
        {
            AllMembershipTypes = new Dictionary<string, int>();
        }

        public int AssociateCount { get; set; }
        public int ChamberAssociateCount { get; set; }
        public int NonLocalIndustrialCount { get; set; }
        public int GovernmentAssociationCount { get; set; }
        public int LocalCount { get; set; }
        public int OtherCount { get; set; }
        
        // Dictionary to store all membership types and their counts
        public Dictionary<string, int> AllMembershipTypes { get; set; }
    }
}
