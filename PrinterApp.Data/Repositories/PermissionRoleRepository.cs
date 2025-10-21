using Microsoft.EntityFrameworkCore;
using PrinterApp.Models.Entities;

namespace PrinterApp.Data.Repositories;

public class PermissionRoleRepository : Repository<PermissionRole>, IPermissionRoleRepository
{
    public PermissionRoleRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<PermissionRole>> GetRolesByPermissionIdAsync(int permissionId)
    {
        return await _dbSet
            .Where(pr => pr.PermissionId == permissionId)
            .ToListAsync();
    }

    public async Task<PermissionRole> GetByPermissionAndRoleNameAsync(int permissionId, string roleName)
    {
        return await _dbSet
            .FirstOrDefaultAsync(pr => pr.PermissionId == permissionId && pr.RoleName == roleName);
    }
}