using Microsoft.EntityFrameworkCore;
using PrinterApp.Data;
using PrinterApp.Models.Entities;

namespace PrinterApp.Data.Repositories
{
    public class OrderTimelineRepository : Repository<OrderTimeline>, IOrderTimelineRepository
    {
        public OrderTimelineRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<OrderTimeline>> GetByOrderIdAsync(int orderId)
        {
            return await _context.OrderTimelines
                .Where(t => t.OrderId == orderId)
                .OrderByDescending(t => t.ActionDate)
                .ToListAsync();
        }

        public async Task<OrderTimeline> GetLatestByOrderIdAsync(int orderId)
        {
            return await _context.OrderTimelines
                .Where(t => t.OrderId == orderId)
                .OrderByDescending(t => t.ActionDate)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<OrderTimeline>> GetByStageAsync(int orderId, OrderStage stage)
        {
            return await _context.OrderTimelines
                .Where(t => t.OrderId == orderId && t.Stage == stage)
                .OrderByDescending(t => t.ActionDate)
                .ToListAsync();
        }

        public async Task AddTimelineEntryAsync(int orderId, OrderStage stage, OrderStatus status, string action, string notes, string userId, string userName)
        {
            var timeline = new OrderTimeline
            {
                OrderId = orderId,
                Stage = stage,
                Status = status,
                Action = action,
                Notes = notes,
                ActionDate = DateTime.Now,
                ActionBy = userId,
                ActionByName = userName
            };

            await AddAsync(timeline);
        }
    }
}