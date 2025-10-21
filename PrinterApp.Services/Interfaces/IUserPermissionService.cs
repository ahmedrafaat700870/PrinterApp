using PrinterApp.Models.ViewModels;
namespace PrinterApp.Services.Interfaces;

public interface IUserPermissionService
{
    Task<UserPermissionsViewModel> GetUserPermissionsAsync(string userId);
    Task<(bool Success, string[] Errors)> UpdateUserPermissionsAsync(string userId, Dictionary<int, List<int>> permissionRoles);
    Task<bool> UserHasPermissionRoleAsync(string userId, string permissionCode, string roleName);
}