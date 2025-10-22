using System.ComponentModel.DataAnnotations;

namespace PrinterApp.Models.Entities;

public class RawMaterial
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Raw material name is required")]
    [StringLength(200, ErrorMessage = "Raw material name cannot exceed 200 characters")]
    [Display(Name = "Raw Material Name")]
    public string RawMaterialName { get; set; }

    [Required(ErrorMessage = "Width is required")]
    [Range(0.01, 100000, ErrorMessage = "Width must be between 0.01 and 100000")]
    [Display(Name = "Width (cm)")]
    public decimal Width { get; set; }

    [Required(ErrorMessage = "Height is required")]
    [Range(0.01, 100000, ErrorMessage = "Height must be between 0.01 and 100000")]
    [Display(Name = "Height (m)")]
    public decimal Height { get; set; }

    [Required(ErrorMessage = "Total price is required")]
    [Range(0.01, 10000000, ErrorMessage = "Total price must be between 0.01 and 10000000")]
    [Display(Name = "Total Price")]
    public decimal TotalPrice { get; set; }

    // Calculated Fields (Auto-calculated)
    [Display(Name = "Area (m²)")]
    public decimal AreaSquareMeters { get; set; }

    [Display(Name = "Price per Square Meter")]
    public decimal PricePerSquareMeter { get; set; }

    [Display(Name = "Price per Linear Meter")]
    public decimal PricePerLinearMeter { get; set; }

    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; }

    [Display(Name = "Last Modified")]
    public DateTime? LastModified { get; set; }

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; } = true;
}