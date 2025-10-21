using PrinterApp.Models.Entities;

namespace PrinterApp.Data.Repositories;

public interface IUserPermissionRepository : IRepository<UserPermission>
{
    Task<List<UserPermission>> GetUserPermissionsAsync(string userId);
    Task<List<UserPermission>> GetUserPermissionsByPermissionIdAsync(string userId, int permissionId);
    Task<bool> UserHasPermissionRoleAsync(string userId, string permissionCode, string roleName);
    Task GrantPermissionRoleAsync(string userId, int permissionId, int permissionRoleId);
    Task RevokePermissionRoleAsync(string userId, int permissionId, int permissionRoleId);
    Task RevokeAllUserPermissionsByPermissionAsync(string userId, int permissionId);
}