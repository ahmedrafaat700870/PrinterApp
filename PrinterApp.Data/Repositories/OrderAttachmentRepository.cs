using Microsoft.EntityFrameworkCore;
using PrinterApp.Data;
using PrinterApp.Models.Entities;

namespace PrinterApp.Data.Repositories
{
    public class OrderAttachmentRepository : Repository<OrderAttachment>, IOrderAttachmentRepository
    {
        public OrderAttachmentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<OrderAttachment>> GetByOrderIdAsync(int orderId)
        {
            return await _context.OrderAttachments
                .Where(a => a.OrderId == orderId && a.IsActive)
                .OrderBy(a => a.UploadedDate)
                .ToListAsync();
        }

        public async Task<OrderAttachment> GetByFileNameAsync(string fileName)
        {
            return await _context.OrderAttachments
                .FirstOrDefaultAsync(a => a.FileName == fileName);
        }

        public async Task<int> GetAttachmentsCountByOrderAsync(int orderId)
        {
            return await _context.OrderAttachments
                .CountAsync(a => a.OrderId == orderId && a.IsActive);
        }

        public async Task<long> GetTotalFileSizeByOrderAsync(int orderId)
        {
            return await _context.OrderAttachments
                .Where(a => a.OrderId == orderId && a.IsActive)
                .SumAsync(a => a.FileSize);
        }
    }
}