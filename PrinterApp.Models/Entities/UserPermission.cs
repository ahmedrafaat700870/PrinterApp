
namespace PrinterApp.Models.Entities;

public class UserPermission
{
    public int Id { get; set; }

    public string UserId { get; set; }

    public int PermissionId { get; set; }

    public int PermissionRoleId { get; set; }

    public DateTime GrantedDate { get; set; }

    // Navigation Properties
    public ApplicationUser User { get; set; }
    public Permission Permission { get; set; }
    public PermissionRole PermissionRole { get; set; }
}