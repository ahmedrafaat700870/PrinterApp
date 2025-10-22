using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PrinterApp.Models.ViewModels;

public class MoldViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "MoldNumberRequired")]
    [StringLength(50, ErrorMessage = "MoldNumberLength")]
    [Display(Name = "MoldNumber")]
    public string MoldNumber { get; set; }

    [Required(ErrorMessage = "MachineRequired")]
    [Display(Name = "Machine")]
    public int MachineId { get; set; }

    [Display(Name = "MachineName")]
    public string MachineName { get; set; }

    [Display(Name = "MachineCode")]
    public string MachineCode { get; set; }

    [Required(ErrorMessage = "MoldShapeRequired")]
    [Display(Name = "MoldShape")]
    public int MoldShapeId { get; set; }

    [Display(Name = "MoldShapeName")]
    public string MoldShapeName { get; set; }

    [Display(Name = "ShapeImagePath")]
    public string ShapeImagePath { get; set; }

    [Required(ErrorMessage = "WidthRequired")]
    [Range(1, 1000, ErrorMessage = "WidthRange")]
    [Display(Name = "Width")]
    public int Width { get; set; }

    [Required(ErrorMessage = "HeightRequired")]
    [Range(1, 1000, ErrorMessage = "HeightRange")]
    [Display(Name = "Height")]
    public int Height { get; set; }

    [Display(Name = "TotalEyes")]
    public int TotalEyes { get; set; }

    [Required(ErrorMessage = "PrintedSizeRequired")]
    [Range(0.01, 10000, ErrorMessage = "PrintedSizeRange")]
    [Display(Name = "PrintedSize")]
    public decimal PrintedRawMaterialSize { get; set; }

    [Required(ErrorMessage = "PlainSizeRequired")]
    [Range(0.01, 10000, ErrorMessage = "PlainSizeRange")]
    [Display(Name = "PlainSize")]
    public decimal PlainRawMaterialSize { get; set; }

    [StringLength(500)]
    [Display(Name = "Description")]
    public string Description { get; set; }

    [Display(Name = "CreatedDate")]
    public DateTime CreatedDate { get; set; }

    [Display(Name = "LastModified")]
    public DateTime? LastModified { get; set; }

    [Display(Name = "IsActive")]
    public bool IsActive { get; set; }

    // For dropdown lists
    public List<SelectListItem> Machines { get; set; }
    public List<SelectListItem> MoldShapes { get; set; }
}