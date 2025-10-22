using PrinterApp.Models.Entities;

namespace PrinterApp.Data.Repositories
{
    public interface IProductAdditionRepository : IRepository<ProductAddition>
    {
        Task<List<ProductAddition>> GetByProductIdAsync(int productId);
        Task DeleteByProductIdAsync(int productId);
    }
}