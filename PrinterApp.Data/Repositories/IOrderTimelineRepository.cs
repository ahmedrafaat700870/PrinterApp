using PrinterApp.Models.Entities;

namespace PrinterApp.Data.Repositories
{
    public interface IOrderTimelineRepository : IRepository<OrderTimeline>
    {
        Task<IEnumerable<OrderTimeline>> GetByOrderIdAsync(int orderId);
        Task<OrderTimeline> GetLatestByOrderIdAsync(int orderId);
        Task<IEnumerable<OrderTimeline>> GetByStageAsync(int orderId, OrderStage stage);
        Task AddTimelineEntryAsync(int orderId, OrderStage stage, OrderStatus status, string action, string notes, string userId, string userName);
    }
}