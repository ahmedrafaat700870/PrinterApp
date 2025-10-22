using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PrinterApp.Models.ViewModels;

public class MoldShapeViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "MoldShapeNameRequired")]
    [StringLength(100, ErrorMessage = "MoldShapeNameLength")]
    [Display(Name = "MoldShapeName")]
    public string ShapeName { get; set; }

    [Display(Name = "ShapeImage")]
    public IFormFile ShapeImage { get; set; }

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
    public bool IsActive { get; set; }

    [Display(Name = "MoldsCount")]
    public int MoldsCount { get; set; }
}