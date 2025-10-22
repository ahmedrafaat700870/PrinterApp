using Microsoft.EntityFrameworkCore;
using PrinterApp.Models.Entities;

namespace PrinterApp.Data.Repositories
{
    public class CartonRepository : Repository<Carton>, ICartonRepository
    {
        public CartonRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Carton>> GetActiveCartonsAsync()
        {
            return await _dbSet
                .Where(c => c.IsActive)
                .OrderBy(c => c.CartonName)
                .ToListAsync();
        }

        public async Task<Carton> GetByNameAsync(string cartonName)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.CartonName == cartonName);
        }

        public async Task<bool> CartonNameExistsAsync(string cartonName, int? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return await _dbSet.AnyAsync(c => c.CartonName == cartonName && c.Id != excludeId.Value);
            }
            return await _dbSet.AnyAsync(c => c.CartonName == cartonName);
        }

        public async Task<List<Carton>> GetByFactorRangeAsync(decimal minFactor, decimal maxFactor)
        {
            return await _dbSet
                .Where(c => c.CartonFactor >= minFactor && c.CartonFactor <= maxFactor)
                .OrderBy(c => c.CartonFactor)
                .ToListAsync();
        }
    }
}