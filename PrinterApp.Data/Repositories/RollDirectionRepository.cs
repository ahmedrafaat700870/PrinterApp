using Microsoft.EntityFrameworkCore;
using PrinterApp.Models.Entities;

namespace PrinterApp.Data.Repositories;

public class RollDirectionRepository : Repository<RollDirection>, IRollDirectionRepository
{
    public RollDirectionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<RollDirection>> GetActiveDirectionsAsync()
    {
        return await _dbSet
            .Where(rd => rd.IsActive)
            .OrderBy(rd => rd.DirectionNumber)
            .ToListAsync();
    }

    public async Task<RollDirection> GetByDirectionNumberAsync(int directionNumber)
    {
        return await _dbSet
            .FirstOrDefaultAsync(rd => rd.DirectionNumber == directionNumber);
    }

    public async Task<bool> DirectionNumberExistsAsync(int directionNumber, int? excludeId = null)
    {
        if (excludeId.HasValue)
        {
            return await _dbSet.AnyAsync(rd => rd.DirectionNumber == directionNumber && rd.Id != excludeId.Value);
        }
        return await _dbSet.AnyAsync(rd => rd.DirectionNumber == directionNumber);
    }
}