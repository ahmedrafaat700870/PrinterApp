using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PrinterApp.Models.Entities;

public class ApplicationUser : IdentityUser
{
    [Required]
    [StringLength(50)]
    public string FirstName { get; set; }

    [Required]
    [StringLength(50)]
    public string LastName { get; set; }

    public string FullName => $"{FirstName} {LastName}";

    // Navigation Properties
    public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
}