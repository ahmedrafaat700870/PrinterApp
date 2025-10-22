using Microsoft.EntityFrameworkCore;
using PrinterApp.Models.Entities;

namespace PrinterApp.Data.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Product>> GetActiveProductsAsync()
        {
            return await _dbSet
                .Include(p => p.RawMaterial)
                .Include(p => p.Supplier)
                .Include(p => p.ProductAdditions)
                    .ThenInclude(pa => pa.ManufacturingAddition)
                .Where(p => p.IsActive)
                .OrderBy(p => p.ProductName)
                .ToListAsync();
        }

        public async Task<List<Product>> GetProductsWithDetailsAsync()
        {
            return await _dbSet
                .Include(p => p.RawMaterial)
                .Include(p => p.Supplier)
                .Include(p => p.ProductAdditions)
                    .ThenInclude(pa => pa.ManufacturingAddition)
                .OrderBy(p => p.ProductName)
                .ToListAsync();
        }

        public async Task<Product> GetProductWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(p => p.RawMaterial)
                .Include(p => p.Supplier)
                .Include(p => p.ProductAdditions)
                    .ThenInclude(pa => pa.ManufacturingAddition)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product> GetByCodeAsync(string productCode)
        {
            return await _dbSet
                .Include(p => p.RawMaterial)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.ProductCode == productCode);
        }

        public async Task<bool> ProductCodeExistsAsync(string productCode, int? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return await _dbSet.AnyAsync(p => p.ProductCode == productCode && p.Id != excludeId.Value);
            }
            return await _dbSet.AnyAsync(p => p.ProductCode == productCode);
        }

        public async Task<bool> ProductNameExistsAsync(string productName, int? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return await _dbSet.AnyAsync(p => p.ProductName == productName && p.Id != excludeId.Value);
            }
            return await _dbSet.AnyAsync(p => p.ProductName == productName);
        }

        public async Task<List<Product>> GetProductsBySupplierAsync(int supplierId)
        {
            return await _dbSet
                .Include(p => p.RawMaterial)
                .Include(p => p.Supplier)
                .Where(p => p.SupplierId == supplierId)
                .OrderBy(p => p.ProductName)
                .ToListAsync();
        }

        public async Task<List<Product>> GetProductsByRawMaterialAsync(int rawMaterialId)
        {
            return await _dbSet
                .Include(p => p.RawMaterial)
                .Include(p => p.Supplier)
                .Where(p => p.RawMaterialId == rawMaterialId)
                .OrderBy(p => p.ProductName)
                .ToListAsync();
        }

        public async Task<string> GetNextProductCodeAsync(int supplierId)
        {
            var supplier = await _context.Suppliers.FindAsync(supplierId);
            if (supplier == null) return null;

            // Get all products for this supplier
            var existingProducts = await _dbSet
                .Where(p => p.SupplierId == supplierId)
                .Select(p => p.ProductCode)
                .ToListAsync();

            // Extract numbers from existing codes
            var numbers = existingProducts
                .Where(code => code.StartsWith(supplier.SupplierCode))
                .Select(code =>
                {
                    var numPart = code.Substring(supplier.SupplierCode.Length);
                    return int.TryParse(numPart, out int num) ? num : 0;
                })
                .ToList();

            // Get next number
            int nextNumber = numbers.Any() ? numbers.Max() + 1 : 1;

            // Format as two digits
            return $"{supplier.SupplierCode}{nextNumber:D2}";
        }
    }
}