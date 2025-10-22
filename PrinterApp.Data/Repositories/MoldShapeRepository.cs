using Microsoft.EntityFrameworkCore;
using PrinterApp.Models.Entities;

namespace PrinterApp.Data.Repositories
{
    public class MoldShapeRepository : Repository<MoldShape>, IMoldShapeRepository
    {
        public MoldShapeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<MoldShape>> GetActiveShapesAsync()
        {
            return await _dbSet
                .Where(s => s.IsActive)
                .OrderBy(s => s.ShapeName)
                .ToListAsync();
        }

        public async Task<MoldShape> GetByNameAsync(string shapeName)
        {
            return await _dbSet
                .FirstOrDefaultAsync(s => s.ShapeName == shapeName);
        }

        public async Task<bool> ShapeNameExistsAsync(string shapeName, int? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return await _dbSet.AnyAsync(s => s.ShapeName == shapeName && s.Id != excludeId.Value);
            }
            return await _dbSet.AnyAsync(s => s.ShapeName == shapeName);
        }

        public async Task<MoldShape> GetShapeWithMoldsAsync(int id)
        {
            return await _dbSet
                .Include(s => s.Molds)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}