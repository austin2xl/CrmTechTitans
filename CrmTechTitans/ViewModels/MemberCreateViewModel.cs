using CrmTechTitans.Models.Enumerations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CrmTechTitans.Models.ViewModels
{
    public class MemberCreateViewModel
    {
        public int ID { get; set; }

        [Display(Name = "Member Name")]
        [Required(ErrorMessage = "Member Name is required")]
        [StringLength(100, ErrorMessage = "Member Name cannot exceed 100 characters")]
        public string MemberName { get; set; }

        [Display(Name = "Membership Type")]
        [Required(ErrorMessage = "Membership Type is required")]
        public List<int> SelectedMembershipTypeIDs { get; set; } = new List<int>();
        public List<MembershipTypeViewModel> AvailableMembershipTypes { get; set; } = new List<MembershipTypeViewModel>();

        [Display(Name = "Contacted By")]
        [Required(ErrorMessage = "Contacted By is required")]
        [StringLength(100, ErrorMessage = "Contacted By cannot exceed 100 characters")]
        public string ContactedBy { get; set; }

        [Display(Name = "Company Size")]
        [Required(ErrorMessage = "Company Size is required")]
        public CompanySize CompanySize { get; set; }

        [Display(Name = "Company Website")]
        [Required(ErrorMessage = "Company Website is required")]
        [StringLength(255, ErrorMessage = "Website cannot exceed 255 characters")]
        public string CompanyWebsite { get; set; }

        [Display(Name = "Member Since")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Member Since is required")]
        public DateTime MemberSince { get; set; } = DateTime.UtcNow;

        [Display(Name = "Last Contact Date")]
        [DataType(DataType.Date)]
        public DateTime? LastContactDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Notes")]
        [Required(ErrorMessage = "Notes are required")]
        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string Notes { get; set; }

        [Display(Name = "Membership Status")]
        [Required(ErrorMessage = "Membership Status is required")]
        public MembershipStatus MembershipStatus { get; set; }

        public MemberPhoto? MemberPhoto { get; set; }
        public MemberThumbnail? MemberThumbnail { get; set; }

        public List<AddressViewModel> Addresses { get; set; } = new List<AddressViewModel>();
        public List<ContactViewModel> Contacts { get; set; } = new List<ContactViewModel>();

        [Display(Name = "Industries")]
        [Required(ErrorMessage = "At least one industry is required")]
        public List<int> SelectedIndustryIds { get; set; } = new List<int>();
        public List<IndustryViewModel> AvailableIndustries { get; set; } = new List<IndustryViewModel>();
    }

    public class AddressViewModel
    {
        public int ID { get; set; }

        [Display(Name = "Street")]
        [Required(ErrorMessage = "Street is required")]
        [StringLength(200, ErrorMessage = "Street cannot exceed 200 characters")]
        public string Street { get; set; }

        [Display(Name = "City")]
        [Required(ErrorMessage = "City is required")]
        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        public string City { get; set; }

        [Display(Name = "Province")]
        [Required(ErrorMessage = "Province is required")]
        public Province Province { get; set; }

        [Display(Name = "Postal Code")]
        [Required(ErrorMessage = "Postal Code is required")]
        [StringLength(20, ErrorMessage = "Postal Code cannot exceed 20 characters")]
        [RegularExpression(@"^[A-Za-z]\d[A-Za-z][ ]?\d[A-Za-z]\d$", ErrorMessage = "Invalid postal code format (A#A #A#)")]
        public string PostalCode { get; set; }

        [Display(Name = "Address Type")]
        [Required(ErrorMessage = "Address Type is required")]
        public AddressType AddressType { get; set; }
    }

    public class ContactViewModel
    {
        public int ID { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name is required")]
        [StringLength(100, ErrorMessage = "First Name cannot exceed 100 characters")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(100, ErrorMessage = "Last Name cannot exceed 100 characters")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
        public string Email { get; set; }

        [Display(Name = "Phone")]
        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be 10 digits")]
        public string Phone { get; set; }

        [Display(Name = "Contact Type")]
        [Required(ErrorMessage = "Contact Type is required")]
        public ContactType ContactType { get; set; }

        public ContactPhoto? ContactPhoto { get; set; }
        public ContactThumbnail? ContactThumbnail { get; set; }
    }

    public class IndustryViewModel
    {
        public int ID { get; set; }

        [Display(Name = "Industry")]
        [Required(ErrorMessage = "Industry is required")]
        [StringLength(100, ErrorMessage = "Industry cannot exceed 100 characters")]
        public string Name { get; set; }

        [Display(Name = "NAICS Code")]
        [Required(ErrorMessage = "NAICS Code is required")]
        public int NAICS { get; set; }
    }

    public class MembershipTypeViewModel
    {
        public int ID { get; set; }

        [Display(Name = "Membership Type")]
        public string Name { get; set; }
    }
}
