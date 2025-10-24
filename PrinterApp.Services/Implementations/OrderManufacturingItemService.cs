using PrinterApp.Data.UnitOfWork;
using PrinterApp.Models.Entities;
using PrinterApp.Models.ViewModels;
using PrinterApp.Services.Interfaces;

namespace PrinterApp.Services.Implementations
{
    public class OrderManufacturingItemService : IOrderManufacturingItemService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderManufacturingItemService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ManufacturingItemViewModel>> GetByOrderIdAsync(int orderId)
        {
            var items = await _unitOfWork.OrderManufacturingItems.GetByOrderIdAsync(orderId);

            return items.OrderBy(mi => mi.DisplayOrder).Select(mi => new ManufacturingItemViewModel
            {
                Id = mi.Id,
                ManufacturingAdditionId = mi.ManufacturingAdditionId,
                AdditionName = mi.ManufacturingAddition?.AdditionName,
                IsCompleted = mi.IsCompleted,
                CompletedDate = mi.CompletedDate,
                CompletedByName = mi.CompletedBy
            });
        }

        public async Task<(bool Success, string[] Errors)> CompleteItemAsync(int id, string userId, string userName)
        {
            var errors = new List<string>();

            try
            {
                var item = await _unitOfWork.OrderManufacturingItems.GetByIdAsync(id);

                if (item == null)
                {
                    errors.Add("العنصر غير موجود");
                    return (false, errors.ToArray());
                }

                if (item.IsCompleted)
                {
                    errors.Add("العنصر مكتمل بالفعل");
                    return (false, errors.ToArray());
                }

                // إتمام العنصر
                item.IsCompleted = true;
                item.CompletedDate = DateTime.Now;
                item.CompletedBy = userId;

                _unitOfWork.OrderManufacturingItems.Update(item);

                // إضافة سجل في Timeline
                var manufacturingAddition = await _unitOfWork.ManufacturingAdditions.GetByIdAsync(item.ManufacturingAdditionId);

                await _unitOfWork.OrderTimelines.AddTimelineEntryAsync(
                    item.OrderId,
                    OrderStage.Manufacturing,
                    OrderStatus.InManufacturing,
                    $"اكتمال عملية: {manufacturingAddition?.AdditionName}",
                    $"تم اكتمال عملية {manufacturingAddition?.AdditionName}",
                    userId,
                    userName
                );

                await _unitOfWork.CompleteAsync();

                return (true, Array.Empty<string>());
            }
            catch (Exception ex)
            {
                errors.Add($"خطأ أثناء إتمام العملية: {ex.Message}");
                return (false, errors.ToArray());
            }
        }

        public async Task<bool> AreAllItemsCompletedAsync(int orderId)
        {
            var items = await _unitOfWork.OrderManufacturingItems.GetByOrderIdAsync(orderId);

            if (!items.Any())
                return false;

            return items.All(mi => mi.IsCompleted);
        }

        public async Task<int> GetCompletedItemsCountAsync(int orderId)
        {
            var items = await _unitOfWork.OrderManufacturingItems.GetByOrderIdAsync(orderId);
            return items.Count(mi => mi.IsCompleted);
        }

        public async Task<int> GetTotalItemsCountAsync(int orderId)
        {
            var items = await _unitOfWork.OrderManufacturingItems.GetByOrderIdAsync(orderId);
            return items.Count();
        }

        public async Task<double> GetCompletionPercentageAsync(int orderId)
        {
            var totalItems = await GetTotalItemsCountAsync(orderId);

            if (totalItems == 0)
                return 0;

            var completedItems = await GetCompletedItemsCountAsync(orderId);
            return Math.Round((double)completedItems / totalItems * 100, 2);
        }
    }
}