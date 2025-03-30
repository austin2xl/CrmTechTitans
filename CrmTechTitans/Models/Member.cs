using CrmTechTitans.Models.Enumerations;
using CrmTechTitans.Models.JoinTables;
using System.ComponentModel.DataAnnotations;

namespace CrmTechTitans.Models
{
    public class Member
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "Member")]
        [Required(ErrorMessage = "Member is required")]
        [StringLength(100, ErrorMessage = "Member name can't be longer than 100 characters")]
        public string MemberName { get; set; }

        
        [Display(Name = "Contacted By")]
        [StringLength(100)]
        public string? ContactedBy { get; set; }


        [Required]
        [Display(Name = "Company Size")]
        public CompanySize CompanySize { get; set; } // Changed to enum

        [Display(Name = "Website")]
        [StringLength(255)]
        public string? CompanyWebsite { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Member Since")]
        public DateTime MemberSince { get; set; } = DateTime.UtcNow;

        [DataType(DataType.DateTime)]
        [Display(Name = "Last Contact Date")]
        public DateTime? LastContactDate { get; set; }

        [StringLength(500)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        public MemberPhoto? MemberPhoto { get; set; }

        public MemberThumbnail? MemberThumbnail { get; set; }

        [Display(Name = "Membership Status")]
        public MembershipStatus MembershipStatus { get; set; }

        [Required]
        [Display(Name = "Membership Type")]
        public ICollection<MemberMembershipType> MemberMembershipTypes { get; set; } = new HashSet<MemberMembershipType>();

        public string Reason { get; set; } = string.Empty;

        public ICollection<MemberIndustry> IndustryMembers { get; set; } = new HashSet<MemberIndustry>();
        public ICollection<MemberAddress> MemberAddresses { get; set; } = new HashSet<MemberAddress>(); // Added relationship -Braydon Pew 01-22-2025

        public ICollection<MemberContact> MemberContacts { get; set; } = new HashSet<MemberContact>();

        public ICollection<MemberOpportunity> MemberOpportunities { get; set; } = new HashSet<MemberOpportunity>();

        public ICollection<InteractionMember> InteractionMembers { get; set; } = new HashSet<InteractionMember>();
    }
}
