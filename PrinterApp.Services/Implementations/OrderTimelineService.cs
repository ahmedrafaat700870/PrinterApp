using PrinterApp.Data.UnitOfWork;
using PrinterApp.Models.Entities;
using PrinterApp.Models.ViewModels;
using PrinterApp.Services.Interfaces;

namespace PrinterApp.Services.Implementations
{
    public class OrderTimelineService : IOrderTimelineService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderTimelineService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<OrderTimelineViewModel>> GetByOrderIdAsync(int orderId)
        {
            var timeline = await _unitOfWork.OrderTimelines.GetByOrderIdAsync(orderId);

            return timeline.OrderByDescending(t => t.ActionDate).Select(t => new OrderTimelineViewModel
            {
                Id = t.Id,
                Stage = t.Stage,
                Status = t.Status,
                Action = t.Action,
                Notes = t.Notes,
                ActionDate = t.ActionDate,
                ActionByName = t.ActionByName
            });
        }

        public async Task AddTimelineEntryAsync(int orderId, OrderStage stage, OrderStatus status,
            string action, string notes, string userId, string userName)
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

            await _unitOfWork.OrderTimelines.AddAsync(timeline);
            await _unitOfWork.CompleteAsync();
        }
    }
}