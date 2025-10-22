using System.ComponentModel.DataAnnotations;

namespace PrinterApp.Models.Entities
{
    public class Supplier
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Supplier code is required")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Supplier code must be exactly 4 digits")]
        [Display(Name = "Supplier Code")]
        public string SupplierCode { get; set; }

        [Required(ErrorMessage = "Supplier name is required")]
        [StringLength(200, ErrorMessage = "Supplier name cannot exceed 200 characters")]
        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; }

        [StringLength(50)]
        [Display(Name = "Card Number")]
        public string CardNumber { get; set; }

        [StringLength(50)]
        [Display(Name = "Commercial Register")]
        public string CommercialRegister { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [StringLength(500)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [StringLength(100)]
        [Display(Name = "City")]
        public string City { get; set; }

        [StringLength(100)]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [StringLength(500)]
        [Display(Name = "Notes")]
        public string Notes { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Last Modified")]
        public DateTime? LastModified { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;
    }
}