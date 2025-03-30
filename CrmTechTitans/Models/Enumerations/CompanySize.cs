using System.ComponentModel.DataAnnotations;

namespace CrmTechTitans.Models.Enumerations
{
    public enum CompanySize
    {
        [Display(Name = "1-10")]
        OneToTen,
        
        [Display(Name = "11-50")]
        ElevenToFifty,
        
        [Display(Name = "51-200")]
        FiftyOneToTwoHundred,
        
        [Display(Name = "201-1000")]
        TwoHundredOneToThousand,
        
        [Display(Name = "1000+")]
        ThousandPlus
    }
}
