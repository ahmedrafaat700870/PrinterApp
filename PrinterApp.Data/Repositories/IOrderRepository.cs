using PrinterApp.Models.Entities;

namespace PrinterApp.Data.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        // البحث والفلترة
        Task<Order> GetByOrderNumberAsync(string orderNumber);
        Task<bool> OrderNumberExistsAsync(string orderNumber, int? excludeId = null);
        Task<Order> GetWithAllDetailsAsync(int id);
        Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status);
        Task<IEnumerable<Order>> GetByStageAsync(OrderStage stage);
        Task<IEnumerable<Order>> GetByCustomerAsync(int customerId);
        Task<IEnumerable<Order>> GetBySupplierAsync(int supplierId);
        Task<IEnumerable<Order>> GetByProductAsync(int productId);
        Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Order>> SearchAsync(string searchTerm);

        // الطلبات حسب المرحلة
        Task<IEnumerable<Order>> GetPendingOrdersAsync();
        Task<IEnumerable<Order>> GetReviewOrdersAsync();
        Task<IEnumerable<Order>> GetManufacturingOrdersAsync();
        Task<IEnumerable<Order>> GetPrintingOrdersAsync();
        Task<IEnumerable<Order>> GetCompletedOrdersAsync();

        // الإحصائيات
        Task<int> GetTotalOrdersCountAsync();
        Task<int> GetActiveOrdersCountAsync();
        Task<int> GetOrdersCountByStatusAsync(OrderStatus status);
        Task<int> GetOrdersCountByStageAsync(OrderStage stage);
        Task<IEnumerable<Order>> GetLateOrdersAsync();
        Task<IEnumerable<Order>> GetTodayOrdersAsync();

        // العمليات على المراحل
        Task<bool> MoveToNextStageAsync(int orderId, string userId);
        Task<bool> UpdateStageAsync(int orderId, OrderStage stage, string userId);
        Task<bool> UpdateStatusAsync(int orderId, OrderStatus status, string userId);
    }
}