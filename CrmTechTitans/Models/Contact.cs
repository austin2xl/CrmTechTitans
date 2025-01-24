using CrmTechTitans.Models.JoinTables;
using System.ComponentModel.DataAnnotations;

namespace CrmTechTitans.Models
{
    public class Contact
    {
        public int ID { get; set; }

        #region FullName Summary

        [Display(Name = "Full Name")]
        public string FullFormalName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }
        #endregion

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
        [RegularExpression("^\\d{10}$", ErrorMessage = "Please enter a valid 10-digit phone number (no spaces).")]
        [DataType(DataType.PhoneNumber)]
        [MaxLength(10)]
        public string? Phone { get; set; }

        [Display(Name = "Linkedin")]
        [StringLength(100, ErrorMessage = "Linkedin can't be longer than 100 characters")]
        public string? Linkedin { get; set; }

        public ICollection<MemberContact> MemberContacts { get; set; } = new HashSet<MemberContact>();

    }
}
