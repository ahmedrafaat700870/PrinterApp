using PrinterApp.Models.Entities;
using PrinterApp.Models.ViewModels;

namespace PrinterApp.Services.Interfaces
{
    public interface IOrderTimelineService
    {
        Task<IEnumerable<OrderTimelineViewModel>> GetByOrderIdAsync(int orderId);
        Task AddTimelineEntryAsync(int orderId, OrderStage stage, OrderStatus status,
            string action, string notes, string userId, string userName);
    }
}