using Microsoft.EntityFrameworkCore;
using PrinterApp.Models.Entities;

namespace PrinterApp.Data.Repositories
{
    public class ProductAdditionRepository : Repository<ProductAddition>, IProductAdditionRepository
    {
        public ProductAdditionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<ProductAddition>> GetByProductIdAsync(int productId)
        {
            return await _dbSet
                .Include(pa => pa.ManufacturingAddition)
                .Where(pa => pa.ProductId == productId)
                .ToListAsync();
        }

        public async Task DeleteByProductIdAsync(int productId)
        {
            var productAdditions = await _dbSet
                .Where(pa => pa.ProductId == productId)
                .ToListAsync();

            _dbSet.RemoveRange(productAdditions);
        }
    }
}