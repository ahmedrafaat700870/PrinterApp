
namespace PrinterApp.Models.ViewModels;

public class UserPermissionsViewModel
{
    public string UserId { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public List<PermissionWithRolesViewModel> Permissions { get; set; } = new List<PermissionWithRolesViewModel>();
}

public class PermissionWithRolesViewModel
{
    public int PermissionId { get; set; }
    public string PermissionName { get; set; }
    public string PermissionCode { get; set; }
    public string Description { get; set; }
    public List<RoleCheckViewModel> Roles { get; set; } = new List<RoleCheckViewModel>();
}

public class RoleCheckViewModel
{
    public int PermissionRoleId { get; set; }
    public string RoleName { get; set; }
    public string Description { get; set; }
    public bool IsGranted { get; set; }
}