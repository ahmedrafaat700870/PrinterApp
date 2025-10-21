using PrinterApp.Models.Entities;
namespace PrinterApp.Data.Repositories;

public interface IPermissionRepository : IRepository<Permission>
{
    Task<Permission> GetByNameAsync(string name);
    Task<bool> ExistsAsync(string name);
}
