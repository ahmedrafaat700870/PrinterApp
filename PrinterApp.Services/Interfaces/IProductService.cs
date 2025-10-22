using PrinterApp.Models.ViewModels;

namespace PrinterApp.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductViewModel>> GetAllProductsAsync();
        Task<IEnumerable<ProductViewModel>> GetActiveProductsAsync();
        Task<IEnumerable<ProductViewModel>> SearchProductsAsync(string searchTerm);
        Task<ProductViewModel> GetProductByIdAsync(int id);
        Task<ProductViewModel> GetProductForCreateAsync();
        Task<ProductViewModel> GetProductForEditAsync(int id);
        Task<(bool Success, string[] Errors)> CreateProductAsync(ProductViewModel model);
        Task<(bool Success, string[] Errors)> UpdateProductAsync(ProductViewModel model);
        Task<(bool Success, string[] Errors)> DeleteProductAsync(int id);
        Task<(bool Success, string[] Errors)> ToggleProductStatusAsync(int id);
        Task<string> GetNextProductCodeAsync(int supplierId);
    }
}