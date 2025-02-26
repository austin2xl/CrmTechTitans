using CrmTechTitans.Models;

namespace CrmTechTitans.ViewModels
{
    public class MemberExportViewModel
    {
        public IEnumerable<Member> Members { get; set; } = new List<Member>();
    }
}
