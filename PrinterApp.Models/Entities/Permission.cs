using System.ComponentModel.DataAnnotations;
namespace PrinterApp.Models.Entities;
public class Permission
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } // مثال: User Management

    [StringLength(500)]
    public string Description { get; set; }

    [Required]
    [StringLength(100)]
    public string Code { get; set; } // مثال: USER_MANAGEMENT

    public DateTime CreatedDate { get; set; }

    // Navigation Properties
    public ICollection<PermissionRole> PermissionRoles { get; set; } = new List<PermissionRole>();
    public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
}