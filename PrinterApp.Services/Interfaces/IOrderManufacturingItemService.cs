using PrinterApp.Models.ViewModels;

namespace PrinterApp.Services.Interfaces
{
    public interface IOrderManufacturingItemService
    {
        Task<IEnumerable<ManufacturingItemViewModel>> GetByOrderIdAsync(int orderId);
        Task<(bool Success, string[] Errors)> CompleteItemAsync(int id, string userId, string userName);
        Task<bool> AreAllItemsCompletedAsync(int orderId);
        Task<int> GetCompletedItemsCountAsync(int orderId);
        Task<int> GetTotalItemsCountAsync(int orderId);
        Task<double> GetCompletionPercentageAsync(int orderId);
    }
}