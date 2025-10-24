using Microsoft.EntityFrameworkCore;
using PrinterApp.Data;
using PrinterApp.Models.Entities;

namespace PrinterApp.Data.Repositories
{
    public class OrderManufacturingItemRepository : Repository<OrderManufacturingItem>, IOrderManufacturingItemRepository
    {
        public OrderManufacturingItemRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<OrderManufacturingItem>> GetByOrderIdAsync(int orderId)
        {
            return await _context.OrderManufacturingItems
                .Include(mi => mi.ManufacturingAddition)
                .Where(mi => mi.OrderId == orderId)
                .OrderBy(mi => mi.DisplayOrder)
                .ToListAsync();
        }

        public async Task<OrderManufacturingItem> GetByOrderAndAdditionAsync(int orderId, int additionId)
        {
            return await _context.OrderManufacturingItems
                .Include(mi => mi.ManufacturingAddition)
                .FirstOrDefaultAsync(mi => mi.OrderId == orderId && mi.ManufacturingAdditionId == additionId);
        }

        public async Task<bool> CompleteItemAsync(int id, string userId)
        {
            var item = await GetByIdAsync(id);
            if (item == null)
                return false;

            item.IsCompleted = true;
            item.CompletedDate = DateTime.Now;
            item.CompletedBy = userId;

            _context.OrderManufacturingItems.Update(item);
            return true;
        }

        public async Task<bool> AreAllItemsCompletedAsync(int orderId)
        {
            return await _context.OrderManufacturingItems
                .Where(mi => mi.OrderId == orderId)
                .AllAsync(mi => mi.IsCompleted);
        }

        public async Task<int> GetCompletedItemsCountAsync(int orderId)
        {
            return await _context.OrderManufacturingItems
                .CountAsync(mi => mi.OrderId == orderId && mi.IsCompleted);
        }

        public async Task<int> GetTotalItemsCountAsync(int orderId)
        {
            return await _context.OrderManufacturingItems
                .CountAsync(mi => mi.OrderId == orderId);
        }

        public async Task<double> GetCompletionPercentageAsync(int orderId)
        {
            var total = await GetTotalItemsCountAsync(orderId);
            if (total == 0)
                return 0;

            var completed = await GetCompletedItemsCountAsync(orderId);
            return (double)completed / total * 100;
        }
    }
}