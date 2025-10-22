using System.ComponentModel.DataAnnotations;

namespace PrinterApp.Models.Entities
{
    public class Machine
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Machine name is required")]
        [StringLength(200, ErrorMessage = "Machine name cannot exceed 200 characters")]
        [Display(Name = "Machine Name")]
        public string MachineName { get; set; }

        [Required(ErrorMessage = "Maximum width is required")]
        [Range(0.01, 100000, ErrorMessage = "Maximum width must be between 0.01 and 100000")]
        [Display(Name = "Maximum Width (mm)")]
        public decimal MaxWidth { get; set; }

        [StringLength(500)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [StringLength(100)]
        [Display(Name = "Model Number")]
        public string ModelNumber { get; set; }

        [StringLength(100)]
        [Display(Name = "Manufacturer")]
        public string Manufacturer { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Last Modified")]
        public DateTime? LastModified { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;
    }
}