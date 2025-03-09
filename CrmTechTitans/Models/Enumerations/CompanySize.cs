using System.ComponentModel.DataAnnotations;

namespace CrmTechTitans.Models.Enumerations
{
    public enum CompanySize
    {
        [Display(Name = "1-10 employees")]
        Micro,
        [Display(Name = "11-50 employees")]
        Small,
        [Display(Name = "51-200 employees")]
        Medium,
        [Display(Name = "201-1000 employees")]
        Large,
        [Display(Name = "1000+ employees")]
        Enterprise
    }
}
