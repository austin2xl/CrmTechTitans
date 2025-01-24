using System.ComponentModel.DataAnnotations;

namespace CrmTechTitans.Models.Enumerations
{
    public enum Status
    {
        Qualification,
        Negotiating,
        [Display(Name = "Closed - New Member")]
        ClosedNewMember,
        [Display(Name = "Closed - Not Intersted")]
        ClosedNotInterested

    }
}
