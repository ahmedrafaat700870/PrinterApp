
using PrinterApp.Models.Entities;

namespace PrinterApp.Data.Repositories;

public interface IPermissionRoleRepository : IRepository<PermissionRole>
{
    Task<List<PermissionRole>> GetRolesByPermissionIdAsync(int permissionId);
    Task<PermissionRole> GetByPermissionAndRoleNameAsync(int permissionId, string roleName);
}