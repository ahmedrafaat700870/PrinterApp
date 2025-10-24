using PrinterApp.Models.Entities;

namespace PrinterApp.Data.Repositories
{
    public interface IOrderManufacturingItemRepository : IRepository<OrderManufacturingItem>
    {
        Task<IEnumerable<OrderManufacturingItem>> GetByOrderIdAsync(int orderId);
        Task<OrderManufacturingItem> GetByOrderAndAdditionAsync(int orderId, int additionId);
        Task<bool> CompleteItemAsync(int id, string userId);
        Task<bool> AreAllItemsCompletedAsync(int orderId);
        Task<int> GetCompletedItemsCountAsync(int orderId);
        Task<int> GetTotalItemsCountAsync(int orderId);
        Task<double> GetCompletionPercentageAsync(int orderId);
    }
}