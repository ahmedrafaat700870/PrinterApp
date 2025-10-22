using Microsoft.EntityFrameworkCore;
using PrinterApp.Models.Entities;

namespace PrinterApp.Data.Repositories
{
    public class MachineRepository : Repository<Machine>, IMachineRepository
    {
        public MachineRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Machine>> GetActiveMachinesAsync()
        {
            return await _dbSet
                .Where(m => m.IsActive)
                .OrderBy(m => m.MachineName)
                .ToListAsync();
        }

        public async Task<Machine> GetByNameAsync(string machineName)
        {
            return await _dbSet
                .FirstOrDefaultAsync(m => m.MachineName == machineName);
        }

        public async Task<bool> MachineNameExistsAsync(string machineName, int? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return await _dbSet.AnyAsync(m => m.MachineName == machineName && m.Id != excludeId.Value);
            }
            return await _dbSet.AnyAsync(m => m.MachineName == machineName);
        }

        public async Task<List<Machine>> GetByManufacturerAsync(string manufacturer)
        {
            return await _dbSet
                .Where(m => m.Manufacturer == manufacturer)
                .OrderBy(m => m.MachineName)
                .ToListAsync();
        }
    }
}