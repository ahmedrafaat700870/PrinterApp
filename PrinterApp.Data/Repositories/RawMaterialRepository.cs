using Microsoft.EntityFrameworkCore;
using PrinterApp.Models.Entities;

namespace PrinterApp.Data.Repositories;

public class RawMaterialRepository : Repository<RawMaterial>, IRawMaterialRepository
{
    public RawMaterialRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<RawMaterial>> GetActiveRawMaterialsAsync()
    {
        return await _dbSet
            .Where(r => r.IsActive)
            .OrderBy(r => r.RawMaterialName)
            .ToListAsync();
    }

    public async Task<RawMaterial> GetByNameAsync(string rawMaterialName)
    {
        return await _dbSet
            .FirstOrDefaultAsync(r => r.RawMaterialName == rawMaterialName);
    }

    public async Task<bool> RawMaterialNameExistsAsync(string rawMaterialName, int? excludeId = null)
    {
        if (excludeId.HasValue)
        {
            return await _dbSet.AnyAsync(r => r.RawMaterialName == rawMaterialName && r.Id != excludeId.Value);
        }
        return await _dbSet.AnyAsync(r => r.RawMaterialName == rawMaterialName);
    }
}