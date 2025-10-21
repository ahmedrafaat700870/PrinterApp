using System.ComponentModel.DataAnnotations;


namespace PrinterApp.Models.Entities;

public class PermissionRole
{
    public int Id { get; set; }

    public int PermissionId { get; set; }

    [Required]
    [StringLength(50)]
    public string RoleName { get; set; } // View, Create, Edit, Delete

    [StringLength(200)]
    public string Description { get; set; }

    public DateTime CreatedDate { get; set; }

    // Navigation Properties
    public Permission Permission { get; set; }
    public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
}