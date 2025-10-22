using Microsoft.EntityFrameworkCore;
using PrinterApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrinterApp.Data.Repositories;

public class CoreRepository : Repository<Core> , ICoreRepository
{
    private readonly ApplicationDbContext _context;
    public CoreRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<bool> CoreNameExistsAsync(string coreName, int? excludeId = null)
    {
        if(excludeId.HasValue)
            return await _context.Cores
                .AnyAsync(c => c.CoreName == coreName && c.Id != excludeId.Value);
        else
            return await _context.Cores
                .AnyAsync(c => c.CoreName == coreName);
    }

    public async Task<List<Core>> GetActiveCoresAsync()
    {
        return await _dbSet.Where(x => x.IsActive).OrderBy(x =>x.CoreName).ToListAsync();
    }

    public async Task<Core> GetCoreByName(string coreName)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.CoreName == coreName);
    }
}
