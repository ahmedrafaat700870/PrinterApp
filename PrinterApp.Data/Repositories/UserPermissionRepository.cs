using Microsoft.EntityFrameworkCore;
using PrinterApp.Models.Entities;

namespace PrinterApp.Data.Repositories;

public class UserPermissionRepository : Repository<UserPermission>, IUserPermissionRepository
{
    public UserPermissionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<UserPermission>> GetUserPermissionsAsync(string userId)
    {
        return await _dbSet
            .Include(up => up.Permission)
            .Include(up => up.PermissionRole)
            .Where(up => up.UserId == userId)
            .ToListAsync();
    }

    public async Task<List<UserPermission>> GetUserPermissionsByPermissionIdAsync(string userId, int permissionId)
    {
        return await _dbSet
            .Include(up => up.PermissionRole)
            .Where(up => up.UserId == userId && up.PermissionId == permissionId)
            .ToListAsync();
    }

    public async Task<bool> UserHasPermissionRoleAsync(string userId, string permissionCode, string roleName)
    {
        return await _dbSet
            .Include(up => up.Permission)
            .Include(up => up.PermissionRole)
            .AnyAsync(up => up.UserId == userId
                && up.Permission.Code == permissionCode
                && up.PermissionRole.RoleName == roleName);
    }

    public async Task GrantPermissionRoleAsync(string userId, int permissionId, int permissionRoleId)
    {
        var exists = await _dbSet.AnyAsync(up =>
            up.UserId == userId
            && up.PermissionId == permissionId
            && up.PermissionRoleId == permissionRoleId);

        if (!exists)
        {
            var userPermission = new UserPermission
            {
                UserId = userId,
                PermissionId = permissionId,
                PermissionRoleId = permissionRoleId,
                GrantedDate = DateTime.Now
            };

            await _dbSet.AddAsync(userPermission);
        }
    }

    public async Task RevokePermissionRoleAsync(string userId, int permissionId, int permissionRoleId)
    {
        var userPermission = await _dbSet
            .FirstOrDefaultAsync(up =>
                up.UserId == userId
                && up.PermissionId == permissionId
                && up.PermissionRoleId == permissionRoleId);

        if (userPermission != null)
        {
            _dbSet.Remove(userPermission);
        }
    }

    public async Task RevokeAllUserPermissionsByPermissionAsync(string userId, int permissionId)
    {
        var userPermissions = await _dbSet
            .Where(up => up.UserId == userId && up.PermissionId == permissionId)
            .ToListAsync();

        _dbSet.RemoveRange(userPermissions);
    }
}