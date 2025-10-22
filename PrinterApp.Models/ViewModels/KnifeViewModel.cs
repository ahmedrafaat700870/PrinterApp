using System.ComponentModel.DataAnnotations;

namespace PrinterApp.Models.ViewModels
{
    public class KnifeViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Knife name is required")]
        [StringLength(200, ErrorMessage = "Knife name cannot exceed 200 characters")]
        [Display(Name = "Knife Name")]
        public string KnifeName { get; set; }

        [Required(ErrorMessage = "Knife factor is required")]
        [Range(0.01, 10000, ErrorMessage = "Knife factor must be between 0.01 and 10000")]
        [Display(Name = "Knife Factor")]
        public decimal KnifeFactor { get; set; }

        [StringLength(500)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Last Modified")]
        public DateTime? LastModified { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
    }
}