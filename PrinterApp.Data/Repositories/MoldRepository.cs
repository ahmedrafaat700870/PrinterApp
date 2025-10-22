using Microsoft.EntityFrameworkCore;
using PrinterApp.Models.Entities;

namespace PrinterApp.Data.Repositories
{
    public class MoldRepository : Repository<Mold>, IMoldRepository
    {
        public MoldRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Mold>> GetActiveMoldsAsync()
        {
            return await _dbSet
                .Include(m => m.Machine)
                .Include(m => m.MoldShape)
                .Where(m => m.IsActive)
                .OrderBy(m => m.MoldNumber)
                .ToListAsync();
        }

        public async Task<List<Mold>> GetMoldsWithDetailsAsync()
        {
            return await _dbSet
                .Include(m => m.Machine)
                .Include(m => m.MoldShape)
                .OrderBy(m => m.MoldNumber)
                .ToListAsync();
        }

        public async Task<Mold> GetMoldWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(m => m.Machine)
                .Include(m => m.MoldShape)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Mold> GetByNumberAsync(string moldNumber)
        {
            return await _dbSet
                .Include(m => m.Machine)
                .Include(m => m.MoldShape)
                .FirstOrDefaultAsync(m => m.MoldNumber == moldNumber);
        }

        public async Task<bool> MoldNumberExistsAsync(string moldNumber, int? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return await _dbSet.AnyAsync(m => m.MoldNumber == moldNumber && m.Id != excludeId.Value);
            }
            return await _dbSet.AnyAsync(m => m.MoldNumber == moldNumber);
        }

        public async Task<List<Mold>> GetMoldsByMachineAsync(int machineId)
        {
            return await _dbSet
                .Include(m => m.Machine)
                .Include(m => m.MoldShape)
                .Where(m => m.MachineId == machineId)
                .OrderBy(m => m.MoldNumber)
                .ToListAsync();
        }

        public async Task<List<Mold>> GetMoldsByShapeAsync(int shapeId)
        {
            return await _dbSet
                .Include(m => m.Machine)
                .Include(m => m.MoldShape)
                .Where(m => m.MoldShapeId == shapeId)
                .OrderBy(m => m.MoldNumber)
                .ToListAsync();
        }
    }
}