

using System.ComponentModel.DataAnnotations;

namespace PrinterApp.Models.ViewModels;

public class PermissionRoleViewModel
{
    public int Id { get; set; }

    public int PermissionId { get; set; }

    [Required(ErrorMessage = "Role name is required")]
    [Display(Name = "Role Name")]
    public string RoleName { get; set; }

    [Display(Name = "Description")]
    public string Description { get; set; }

    public DateTime CreatedDate { get; set; }
}