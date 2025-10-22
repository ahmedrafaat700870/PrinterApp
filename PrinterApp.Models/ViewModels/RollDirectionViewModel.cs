using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;


namespace PrinterApp.Models.ViewModels;

public class RollDirectionViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Direction number is required")]
    [Range(1, 1000, ErrorMessage = "Direction number must be between 1 and 1000")]
    [Display(Name = "Direction Number")]
    public int DirectionNumber { get; set; }

    [Display(Name = "Current Image")]
    public string DirectionImage { get; set; }

    [Display(Name = "Direction Image")]
    public IFormFile ImageFile { get; set; }

    [StringLength(200)]
    [Display(Name = "Description")]
    public string Description { get; set; }

    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; }

    [Display(Name = "Last Modified")]
    public DateTime? LastModified { get; set; }

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; }
}