using Microsoft.AspNetCore.Http;
using PrinterApp.Models.Entities;
using PrinterApp.Models.ViewModels;

namespace PrinterApp.Services.Interfaces
{
    public interface IOrderService
    {
        // العمليات الأساسية
        Task<IEnumerable<OrderViewModel>> GetAllOrdersAsync();
        Task<IEnumerable<OrderViewModel>> GetActiveOrdersAsync();
        Task<OrderViewModel> GetOrderByIdAsync(int id);
        Task<OrderViewModel> GetOrderByNumberAsync(string orderNumber);
        Task<OrderDetailsViewModel> GetOrderWithAllDetailsAsync(int id);
        Task<bool> OrderNumberExistsAsync(string orderNumber, int? excludeId = null);

        // مرحلة الطلب (Stage 1)
        Task<(bool Success, string[] Errors)> CreateOrderStage1Async(OrderStage1ViewModel model, IFormFileCollection files, string userId, string userName);
        Task<(bool Success, string[] Errors)> UpdateOrderStage1Async(OrderStage1ViewModel model, IFormFileCollection files, string userId, string userName);

        // مرحلة المراجعة (Stage 2)
        Task<(bool Success, string[] Errors)> MoveToReviewAsync(int orderId, string userId, string userName);
        Task<IEnumerable<OrderViewModel>> GetReviewOrdersAsync();
        Task<OrderStage2ViewModel> GetOrderForReviewAsync(int id);
        Task<(bool Success, string[] Errors)> UpdateOrderStage2Async(OrderStage2ViewModel model, string userId, string userName);
        Task<(bool Success, string[] Errors)> MoveToManufacturingAsync(int orderId, string userId, string userName);

        // مرحلة التصنيع (Stage 3)
        Task<IEnumerable<OrderViewModel>> GetManufacturingOrdersAsync();
        Task<OrderStage3ViewModel> GetOrderForManufacturingAsync(int id);
        Task<(bool Success, string[] Errors)> CompleteManufacturingItemAsync(int orderId, int itemId, string userId, string userName);
        Task<bool> CheckAllManufacturingCompletedAsync(int orderId);
        Task<(bool Success, string[] Errors)> MoveToPrintingAsync(int orderId, string userId, string userName);

        // مرحلة الطباعة (Stage 4)
        Task<IEnumerable<OrderViewModel>> GetPrintingOrdersAsync();
        Task<OrderStage4ViewModel> GetOrderForPrintingAsync(int id);
        Task<(bool Success, string[] Errors)> StartPrintingAsync(int orderId, string userId, string userName);
        Task<(bool Success, string[] Errors)> CompletePrintingAsync(int orderId, string userId, string userName);

        // البحث والفلترة
        Task<IEnumerable<OrderViewModel>> GetOrdersByStatusAsync(OrderStatus status);
        Task<IEnumerable<OrderViewModel>> GetOrdersByStageAsync(OrderStage stage);
        Task<IEnumerable<OrderViewModel>> GetOrdersByCustomerAsync(int customerId);
        Task<IEnumerable<OrderViewModel>> GetOrdersBySupplierAsync(int supplierId);
        Task<IEnumerable<OrderViewModel>> GetOrdersByProductAsync(int productId);
        Task<IEnumerable<OrderViewModel>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<OrderViewModel>> SearchOrdersAsync(string searchTerm);

        // الإحصائيات
        Task<int> GetTotalOrdersCountAsync();
        Task<int> GetActiveOrdersCountAsync();
        Task<int> GetOrdersCountByStatusAsync(OrderStatus status);
        Task<int> GetOrdersCountByStageAsync(OrderStage stage);
        Task<IEnumerable<OrderViewModel>> GetLateOrdersAsync();
        Task<IEnumerable<OrderViewModel>> GetTodayOrdersAsync();
        Task<OrderStatisticsViewModel> GetOrderStatisticsAsync();

        // تغيير الحالة والمرحلة
        Task<(bool Success, string[] Errors)> ChangeOrderStatusAsync(int id, OrderStatus status, string userId, string userName);
        Task<(bool Success, string[] Errors)> ChangeOrderStageAsync(int id, OrderStage stage, string userId, string userName);
        Task<(bool Success, string[] Errors)> DeleteOrderAsync(int id);
        Task<(bool Success, string[] Errors)> CancelOrderAsync(int id, string reason, string userId, string userName);

        // Timeline
        Task<IEnumerable<OrderTimelineViewModel>> GetOrderTimelineAsync(int orderId);
    }
}