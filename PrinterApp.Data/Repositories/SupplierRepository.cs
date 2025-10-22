using Microsoft.EntityFrameworkCore;
using PrinterApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrinterApp.Data.Repositories;

public class SupplierRepository : Repository<Supplier>, ISupplierRepository
{
    public SupplierRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<Supplier>> GetActiveSuppliersAsync()
    {
        return await _dbSet
            .Where(s => s.IsActive)
            .OrderBy(s => s.SupplierCode)
            .ToListAsync();
    }

    public async Task<Supplier> GetByCodeAsync(string supplierCode)
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.SupplierCode == supplierCode);
    }

    public async Task<Supplier> GetByNameAsync(string supplierName)
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.SupplierName == supplierName);
    }

    public async Task<bool> SupplierCodeExistsAsync(string supplierCode)
    {
        return await _dbSet.AnyAsync(s => s.SupplierCode == supplierCode);
    }

    public async Task<bool> SupplierNameExistsAsync(string supplierName, int? excludeId = null)
    {
        if (excludeId.HasValue)
        {
            return await _dbSet.AnyAsync(s => s.SupplierName == supplierName && s.Id != excludeId.Value);
        }
        return await _dbSet.AnyAsync(s => s.SupplierName == supplierName);
    }

    public async Task<string> GetNextSupplierCodeAsync()
    {
        var lastSupplier = await _dbSet
            .OrderByDescending(s => s.SupplierCode)
            .FirstOrDefaultAsync();

        if (lastSupplier == null)
        {
            return "0001";
        }

        if (int.TryParse(lastSupplier.SupplierCode, out int lastCode))
        {
            int nextCode = lastCode + 1;
            return nextCode.ToString("D4"); // D4 = 4 digits with leading zeros
        }

        return "0001";
    }

    public async Task<string> GetLastSupplierCodeAsync()
    {
        var lastSupplier = await _dbSet
            .OrderByDescending(s => s.SupplierCode)
            .FirstOrDefaultAsync();

        return lastSupplier?.SupplierCode ?? "0000";
    }

    public async Task ReassignSupplierCodesAsync()
    {
        var suppliers = await _dbSet
            .OrderBy(s => s.CreatedDate)
            .ToListAsync();

        int counter = 1;
        foreach (var supplier in suppliers)
        {
            supplier.SupplierCode = counter.ToString("D4");
            counter++;
        }

        await _context.SaveChangesAsync();
    }
}