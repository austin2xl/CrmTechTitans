using CrmTechTitans.Models.JoinTables;
using System.ComponentModel.DataAnnotations;

namespace CrmTechTitans.Models
{
    public class Industry
    {
        public int ID { get; set; }

        [Display(Name = "Industry")]
        [Required(ErrorMessage = "Industry is required")]
        [StringLength(100, ErrorMessage = "Industry can't be longer than 100 characters")]
        public string Name { get; set; }

        [Display(Name = "NAICS Code")]
        [Required(ErrorMessage = "NAICS code is required")]
        public int NAICS { get; set; }

        public ICollection<MemberIndustry> IndustryMembers { get; set; } = new HashSet<MemberIndustry>();
    }
}
