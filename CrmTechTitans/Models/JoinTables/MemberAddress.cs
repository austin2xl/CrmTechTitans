using CrmTechTitans.Models;
using CrmTechTitans.Models.Enumerations;

namespace CrmTechTitans.Models.JoinTables
{
    // MemberAddress join table
    public class MemberAddress
    {
        public int MemberID { get; set; }
        public Member? Member { get; set; }

        public int AddressID { get; set; }
        public Address? Address { get; set; }

        public AddressType AddressType { get; set; }
    }
}
