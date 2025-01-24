using CrmTechTitans.Models.Enumerations;

namespace CrmTechTitans.Models.JoinTables
{
    public class MemberContact
    {
        public int MemberID { get; set; }
        public Member? Member { get; set; }

        public int ContactID { get; set; }
        public Contact? Contact { get; set; }
        public ContactType ContactType { get; set; }
    }
}
