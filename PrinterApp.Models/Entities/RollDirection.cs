using System.ComponentModel.DataAnnotations;

namespace PrinterApp.Models.Entities;

public class RollDirection
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Direction number is required")]
    [Range(1, 1000, ErrorMessage = "Direction number must be between 1 and 1000")]
    [Display(Name = "Direction Number")]
    public int DirectionNumber { get; set; }

    [StringLength(500)]
    [Display(Name = "Direction Image")]
    public string DirectionImage { get; set; }

    [StringLength(200)]
    [Display(Name = "Description")]
    public string Description { get; set; }

    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; }

    [Display(Name = "Last Modified")]
    public DateTime? LastModified { get; set; }

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; } = true;
}