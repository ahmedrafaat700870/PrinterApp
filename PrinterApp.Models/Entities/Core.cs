using System.ComponentModel.DataAnnotations;


namespace PrinterApp.Models.Entities;

public class Core
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Core name is required")]
    [StringLength(100, ErrorMessage = "Core name cannot exceed 100 characters")]
    [Display(Name = "Core Name")]
    public string CoreName { get; set; }

    [Required(ErrorMessage = "Core coefficient is required")]
    [Range(0.01, 1000, ErrorMessage = "Core coefficient must be between 0.01 and 1000")]
    [Display(Name = "Core Coefficient")]
    public decimal CoreCoefficient { get; set; }

    [Required(ErrorMessage = "Core width is required")]
    [Range(0.01, 10000, ErrorMessage = "Width must be between 0.01 and 10000")]
    [Display(Name = "Width (mm)")]
    public decimal WidthCor { get; set; }

    [Required(ErrorMessage = "Core height is required")]
    [Range(0.01, 10000, ErrorMessage = "Height must be between 0.01 and 10000")]
    [Display(Name = "Height (mm)")]
    public decimal HeightCor { get; set; }

    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; }

    [Display(Name = "Last Modified")]
    public DateTime? LastModified { get; set; }

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; } = true;














}