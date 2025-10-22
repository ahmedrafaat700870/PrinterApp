using PrinterApp.Models.Entities;

namespace PrinterApp.Data.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<List<Product>> GetActiveProductsAsync();
        Task<List<Product>> GetProductsWithDetailsAsync();
        Task<Product> GetProductWithDetailsAsync(int id);
        Task<Product> GetByCodeAsync(string productCode);
        Task<bool> ProductCodeExistsAsync(string productCode, int? excludeId = null);
        Task<bool> ProductNameExistsAsync(string productName, int? excludeId = null);
        Task<List<Product>> GetProductsBySupplierAsync(int supplierId);
        Task<List<Product>> GetProductsByRawMaterialAsync(int rawMaterialId);
        Task<string> GetNextProductCodeAsync(int supplierId);
    }
}