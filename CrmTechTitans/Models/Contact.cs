using CrmTechTitans.Models.JoinTables;
using CrmTechTitans.Models.Enumerations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CrmTechTitans.Utilities;

namespace CrmTechTitans.Models
{
    public class Contact
    {
        public int ID { get; set; }

        [Display(Name = "Full Name")]
        public string FullFormalName => $"{FirstName} {LastName}";

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "FirstName is required")]
        [StringLength(100, ErrorMessage = "FirstName can't be longer than 100 characters")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(100, ErrorMessage = "LastName can't be longer than 100 characters")]
        public string? LastName { get; set; }

        [Display(Name = "Email")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please follow the correct email format Example@email.com")]
        [StringLength(255)]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Display(Name = "Phone No.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Please enter a valid 10-digit phone number (no spaces).")]
        [DataType(DataType.PhoneNumber)]
        [MaxLength(10)]
        [Required(ErrorMessage = "Phone number is required")]
        public string? Phone { get; set; }

        [Display(Name = "Formatted Phone")]
        public string FormattedPhone => Phone?.FormatPhoneNumber() ?? string.Empty;

        [Display(Name = "Linkedin")]
        [StringLength(100, ErrorMessage = "Linkedin can't be longer than 100 characters")]
        public string? Linkedin { get; set; }

        // Contact Type property
        [Display(Name = "Contact Type")]
        public ContactType? ContactType { get; set; }

        public ContactPhoto? ContactPhoto { get; set; }
        public ContactThumbnail? ContactThumbnail { get; set; }

        public ICollection<MemberContact> MemberContacts { get; set; } = new HashSet<MemberContact>();
    }
}

//changed 02-24