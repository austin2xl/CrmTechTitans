using CrmTechTitans.Models.Enumerations;
using CrmTechTitans.Models.JoinTables;
using System.ComponentModel.DataAnnotations;

namespace CrmTechTitans.Models
{
    public class Opportunity
    {
        public int ID { get; set; }

        [Display(Name = "Title")]
        [StringLength(200, ErrorMessage = "Opportunity title can't be longer than 200 characters")]
        public string? Title { get; set; }

        public Status Status { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000)]
        public string Description { get; set; }

        [Display(Name = "Priority")]
        public PriorityType Priority { get; set; } //Enum

        public ICollection<MemberOpportunity> MemberOpportunities { get; set; } = new HashSet<MemberOpportunity>();
    }
}
