using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrinterApp.Models.Entities;

public class Mold
{
    public int Id { get; set; }

    [Required(ErrorMessage = "MoldNumberRequired")]
    [StringLength(50, ErrorMessage = "MoldNumberLength")]
    [Display(Name = "MoldNumber")]
    public string MoldNumber { get; set; }

    [Required(ErrorMessage = "MachineRequired")]
    [Display(Name = "Machine")]
    public int MachineId { get; set; }

    [ForeignKey("MachineId")]
    public virtual Machine Machine { get; set; }

    [Required(ErrorMessage = "MoldShapeRequired")]
    [Display(Name = "MoldShape")]
    public int MoldShapeId { get; set; }

    [ForeignKey("MoldShapeId")]
    public virtual MoldShape MoldShape { get; set; }

    [Required(ErrorMessage = "WidthRequired")]
    [Range(1, int.MaxValue, ErrorMessage = "WidthRange")]
    [Display(Name = "Width")]
    public int Width { get; set; }

    [Required(ErrorMessage = "HeightRequired")]
    [Range(1, int.MaxValue, ErrorMessage = "HeightRange")]
    [Display(Name = "Height")]
    public int Height { get; set; }

    [Display(Name = "TotalEyes")]
    public int TotalEyes { get; set; }

    [Required(ErrorMessage = "PrintedSizeRequired")]
    [Range(0.01, double.MaxValue, ErrorMessage = "PrintedSizeRange")]
    [Display(Name = "PrintedSize")]
    public decimal PrintedRawMaterialSize { get; set; }

    [Required(ErrorMessage = "PlainSizeRequired")]
    [Range(0.01, double.MaxValue, ErrorMessage = "PlainSizeRange")]
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
    public bool IsActive { get; set; } = true;
}