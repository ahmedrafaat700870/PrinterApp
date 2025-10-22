using System.ComponentModel.DataAnnotations;

namespace PrinterApp.Models.Entities
{
    public class ManufacturingAddition
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Addition name is required")]
        [StringLength(200, ErrorMessage = "Addition name cannot exceed 200 characters")]
        [Display(Name = "Addition Name")]
        public string AdditionName { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Last Modified")]
        public DateTime? LastModified { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;
    }
}