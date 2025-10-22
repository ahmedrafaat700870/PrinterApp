using Microsoft.EntityFrameworkCore;
using PrinterApp.Models.Entities;

namespace PrinterApp.Data.Repositories
{
    public class KnifeRepository : Repository<Knife>, IKnifeRepository
    {
        public KnifeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Knife>> GetActiveKnivesAsync()
        {
            return await _dbSet
                .Where(k => k.IsActive)
                .OrderBy(k => k.KnifeName)
                .ToListAsync();
        }

        public async Task<Knife> GetByNameAsync(string knifeName)
        {
            return await _dbSet
                .FirstOrDefaultAsync(k => k.KnifeName == knifeName);
        }

        public async Task<bool> KnifeNameExistsAsync(string knifeName, int? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return await _dbSet.AnyAsync(k => k.KnifeName == knifeName && k.Id != excludeId.Value);
            }
            return await _dbSet.AnyAsync(k => k.KnifeName == knifeName);
        }

        public async Task<List<Knife>> GetByFactorRangeAsync(decimal minFactor, decimal maxFactor)
        {
            return await _dbSet
                .Where(k => k.KnifeFactor >= minFactor && k.KnifeFactor <= maxFactor)
                .OrderBy(k => k.KnifeFactor)
                .ToListAsync();
        }
    }
}