using Microsoft.EntityFrameworkCore;
using PrinterApp.Data;
using PrinterApp.Models.Entities;

namespace PrinterApp.Data.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
        }

        // ===== البحث والفلترة =====

        public async Task<Order> GetByOrderNumberAsync(string orderNumber)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Supplier)
                .Include(o => o.Product)
                .Include(o => o.RollDirection)
                .Include(o => o.RawMaterial)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
        }

        public async Task<bool> OrderNumberExistsAsync(string orderNumber, int? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return await _context.Orders
                    .AnyAsync(o => o.OrderNumber == orderNumber && o.Id != excludeId.Value);
            }
            return await _context.Orders.AnyAsync(o => o.OrderNumber == orderNumber);
        }

        public async Task<Order> GetWithAllDetailsAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Supplier)
                .Include(o => o.Product)
                .Include(o => o.RollDirection)
                .Include(o => o.RawMaterial)
                .Include(o => o.Machine)
                .Include(o => o.Core)
                .Include(o => o.Knife)
                .Include(o => o.Carton)
                .Include(o => o.Mold)
                .Include(o => o.Attachments)
                .Include(o => o.ManufacturingItems)
                    .ThenInclude(mi => mi.ManufacturingAddition)
                .Include(o => o.Timeline)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Supplier)
                .Include(o => o.Product)
                .Where(o => o.IsActive && o.Status == status)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetByStageAsync(OrderStage stage)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Supplier)
                .Include(o => o.Product)
                .Where(o => o.IsActive && o.Stage == stage)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetByCustomerAsync(int customerId)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Supplier)
                .Include(o => o.Product)
                .Where(o => o.IsActive && o.CustomerId == customerId)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetBySupplierAsync(int supplierId)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Supplier)
                .Include(o => o.Product)
                .Where(o => o.IsActive && o.SupplierId == supplierId)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetByProductAsync(int productId)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Supplier)
                .Include(o => o.Product)
                .Where(o => o.IsActive && o.ProductId == productId)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Supplier)
                .Include(o => o.Product)
                .Where(o => o.IsActive && o.OrderDate >= startDate && o.OrderDate <= endDate)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> SearchAsync(string searchTerm)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Supplier)
                .Include(o => o.Product)
                .Where(o => o.IsActive &&
                    (o.OrderNumber.Contains(searchTerm) ||
                     o.Customer.CustomerName.Contains(searchTerm) ||
                     o.Supplier.SupplierName.Contains(searchTerm) ||
                     o.Product.ProductName.Contains(searchTerm)))
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }

        // ===== الطلبات حسب المرحلة =====

        public async Task<IEnumerable<Order>> GetPendingOrdersAsync()
        {
            return await GetByStageAsync(OrderStage.Order);
        }

        public async Task<IEnumerable<Order>> GetReviewOrdersAsync()
        {
            return await GetByStageAsync(OrderStage.Review);
        }

        public async Task<IEnumerable<Order>> GetManufacturingOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Supplier)
                .Include(o => o.Product)
                .Include(o => o.ManufacturingItems)
                    .ThenInclude(mi => mi.ManufacturingAddition)
                .Where(o => o.IsActive && o.Stage == OrderStage.Manufacturing)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetPrintingOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Supplier)
                .Include(o => o.Product)
                .Include(o => o.Machine)
                .Include(o => o.Core)
                .Include(o => o.Knife)
                .Include(o => o.Carton)
                .Include(o => o.Mold)
                .Where(o => o.IsActive && o.Stage == OrderStage.Printing)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetCompletedOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Supplier)
                .Include(o => o.Product)
                .Where(o => o.IsActive && o.Stage == OrderStage.Completed)
                .OrderByDescending(o => o.ActualDeliveryDate)
                .ToListAsync();
        }

        // ===== الإحصائيات =====

        public async Task<int> GetTotalOrdersCountAsync()
        {
            return await _context.Orders.CountAsync();
        }

        public async Task<int> GetActiveOrdersCountAsync()
        {
            return await _context.Orders.CountAsync(o => o.IsActive);
        }

        public async Task<int> GetOrdersCountByStatusAsync(OrderStatus status)
        {
            return await _context.Orders.CountAsync(o => o.IsActive && o.Status == status);
        }

        public async Task<int> GetOrdersCountByStageAsync(OrderStage stage)
        {
            return await _context.Orders.CountAsync(o => o.IsActive && o.Stage == stage);
        }

        public async Task<IEnumerable<Order>> GetLateOrdersAsync()
        {
            var today = DateTime.Today;
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Supplier)
                .Include(o => o.Product)
                .Where(o => o.IsActive &&
                    o.ExpectedDeliveryDate < today &&
                    o.Stage != OrderStage.Completed)
                .OrderBy(o => o.ExpectedDeliveryDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetTodayOrdersAsync()
        {
            var today = DateTime.Today;
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Supplier)
                .Include(o => o.Product)
                .Where(o => o.IsActive && o.OrderDate.Date == today)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }

        // ===== العمليات على المراحل =====

        public async Task<bool> MoveToNextStageAsync(int orderId, string userId)
        {
            var order = await GetByIdAsync(orderId);
            if (order == null)
                return false;

            switch (order.Stage)
            {
                case OrderStage.Order:
                    order.Stage = OrderStage.Review;
                    order.Status = OrderStatus.UnderReview;
                    break;

                case OrderStage.Review:
                    order.Stage = OrderStage.Manufacturing;
                    order.Status = OrderStatus.InManufacturing;
                    order.ManufacturingStartDate = DateTime.Now;
                    break;

                case OrderStage.Manufacturing:
                    order.Stage = OrderStage.Printing;
                    order.Status = OrderStatus.InPrinting;
                    order.ManufacturingEndDate = DateTime.Now;
                    break;

                case OrderStage.Printing:
                    order.Stage = OrderStage.Completed;
                    order.Status = OrderStatus.Completed;
                    order.PrintingEndDate = DateTime.Now;
                    order.ActualDeliveryDate = DateTime.Now;
                    break;

                default:
                    return false;
            }

            order.LastModified = DateTime.Now;
            order.ModifiedBy = userId;

            _context.Orders.Update(order);
            return true;
        }

        public async Task<bool> UpdateStageAsync(int orderId, OrderStage stage, string userId)
        {
            var order = await GetByIdAsync(orderId);
            if (order == null)
                return false;

            order.Stage = stage;
            order.LastModified = DateTime.Now;
            order.ModifiedBy = userId;

            _context.Orders.Update(order);
            return true;
        }

        public async Task<bool> UpdateStatusAsync(int orderId, OrderStatus status, string userId)
        {
            var order = await GetByIdAsync(orderId);
            if (order == null)
                return false;

            order.Status = status;
            order.LastModified = DateTime.Now;
            order.ModifiedBy = userId;

            if (status == OrderStatus.Completed && !order.ActualDeliveryDate.HasValue)
            {
                order.ActualDeliveryDate = DateTime.Now;
            }

            _context.Orders.Update(order);
            return true;
        }

        public  async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Supplier)
                .Include(o => o.Product)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            return await GetWithAllDetailsAsync(id);
        }
    }
}