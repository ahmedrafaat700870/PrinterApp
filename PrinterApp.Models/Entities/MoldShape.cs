using System.ComponentModel.DataAnnotations;

namespace PrinterApp.Models.Entities;

public class MoldShape
{
    public int Id { get; set; }

    [Required(ErrorMessage = "MoldShapeNameRequired")]
    [StringLength(100, ErrorMessage = "MoldShapeNameLength")]
    [Display(Name = "MoldShapeName")]
    public string ShapeName { get; set; }

    [StringLength(500)]
    [Display(Name = "ShapeImagePath")]
    public string ShapeImagePath { get; set; }

    [StringLength(500)]
    [Display(Name = "Description")]
    public string Description { get; set; }

    [Display(Name = "CreatedDate")]
    public DateTime CreatedDate { get; set; }

    [Display(Name = "LastModified")]
    public DateTime? LastModified { get; set; }

    [Display(Name = "IsActive")]
    public bool IsActive { get; set; } = true;

    // Navigation property
    public virtual ICollection<Mold> Molds { get; set; }
}