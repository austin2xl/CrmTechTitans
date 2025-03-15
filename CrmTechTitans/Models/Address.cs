using System.ComponentModel.DataAnnotations;
using CrmTechTitans.Models.Enumerations;
using CrmTechTitans.Models.JoinTables;

namespace CrmTechTitans.Models
{
    public class Address
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Street is required")]
        [StringLength(200, ErrorMessage = "Street address can't be longer than 200 characters")]
        public string Street { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(100, ErrorMessage = "City name can't be longer than 100 characters")]
        public string City { get; set; }

        [Required(ErrorMessage = "Province is required")]
        public Province Province { get; set; }


        [StringLength(20, ErrorMessage = "Postal code can't be longer than 20 characters")]
        [RegularExpression(@"^[A-Za-z]\d[A-Za-z][ ]?\d[A-Za-z]\d$", ErrorMessage = "Invalid postal code format (should be in the format 'A#A #A#' )")]
        public string? PostalCode { get; set; }

        // Address Type property
        [Display(Name = "Address Type")]
        public AddressType? AddressType { get; set; }

        // Computed property to display the full address
        public string Summary
        {
            get
            {
                // Format the full address
                string fullAddress = $"{Street}, {City}, {Province}";

                // Add postal code if it exists
                if (!string.IsNullOrEmpty(PostalCode))
                {
                    fullAddress += $", {PostalCode}";
                }

                return fullAddress;
            }
        }

        // New ICollection properties - added 3pm 2025-01-23 b.p.
        public ICollection<MemberAddress> MemberAddresses { get; set; } = new HashSet<MemberAddress>();
    }
}
