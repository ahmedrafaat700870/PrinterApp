using System.ComponentModel.DataAnnotations;

namespace PrinterApp.Models.Entities
{
    public class Carton
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Carton name is required")]
        [StringLength(200, ErrorMessage = "Carton name cannot exceed 200 characters")]
        [Display(Name = "Carton Name")]
        public string CartonName { get; set; }

        [Required(ErrorMessage = "Carton factor is required")]
        [Range(0.01, 10000, ErrorMessage = "Carton factor must be between 0.01 and 10000")]
        [Display(Name = "Carton Factor")]
        public decimal CartonFactor { get; set; }

        [StringLength(500)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Last Modified")]
        public DateTime? LastModified { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;
    }
}