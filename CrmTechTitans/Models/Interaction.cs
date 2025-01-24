using System.ComponentModel.DataAnnotations;

namespace CrmTechTitans.Models
{
    public class Interaction
    {
        public int Id { get; set; }

        [Display(Name = "Interaction")]
        public string? interaction { get; set; }
    }
}
