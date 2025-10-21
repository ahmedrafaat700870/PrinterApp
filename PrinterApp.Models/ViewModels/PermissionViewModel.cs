using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PrinterApp.Models.ViewModels;

public class PermissionViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Permission name is required")]
    [StringLength(100)]
    [Display(Name = "Permission Name")]
    public string Name { get; set; }

    [StringLength(500)]
    [Display(Name = "Description")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Permission code is required")]
    [StringLength(100)]
    [Display(Name = "Permission Code")]
    public string Code { get; set; }

    [Display(Name = "Created Date")]
    public DateTime CreatedDate { get; set; }

    public List<PermissionRoleViewModel> Roles { get; set; } = new List<PermissionRoleViewModel>();
}