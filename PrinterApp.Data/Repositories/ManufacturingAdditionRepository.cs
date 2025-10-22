using Microsoft.EntityFrameworkCore;
using PrinterApp.Models.Entities;

namespace PrinterApp.Data.Repositories
{
    public class ManufacturingAdditionRepository : Repository<ManufacturingAddition>, IManufacturingAdditionRepository
    {
        public ManufacturingAdditionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<ManufacturingAddition>> GetActiveAdditionsAsync()
        {
            return await _dbSet
                .Where(a => a.IsActive)
                .OrderBy(a => a.AdditionName)
                .ToListAsync();
        }

        public async Task<ManufacturingAddition> GetByNameAsync(string additionName)
        {
            return await _dbSet
                .FirstOrDefaultAsync(a => a.AdditionName == additionName);
        }

        public async Task<bool> AdditionNameExistsAsync(string additionName, int? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return await _dbSet.AnyAsync(a => a.AdditionName == additionName && a.Id != excludeId.Value);
            }
            return await _dbSet.AnyAsync(a => a.AdditionName == additionName);
        }
    }
}