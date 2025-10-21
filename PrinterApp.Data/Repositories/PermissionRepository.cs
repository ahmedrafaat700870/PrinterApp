using Microsoft.EntityFrameworkCore;
using PrinterApp.Models.Entities;
namespace PrinterApp.Data.Repositories;

public class PermissionRepository : Repository<Permission>, IPermissionRepository
{
    public PermissionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Permission> GetByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.Name == name);
    }

    public async Task<bool> ExistsAsync(string name)
    {
        return await _dbSet.AnyAsync(p => p.Name == name);
    }
}
