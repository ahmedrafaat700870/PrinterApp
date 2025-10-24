using PrinterApp.Models.Entities;

namespace PrinterApp.Data.Repositories
{
    public interface IOrderAttachmentRepository : IRepository<OrderAttachment>
    {
        Task<IEnumerable<OrderAttachment>> GetByOrderIdAsync(int orderId);
        Task<OrderAttachment> GetByFileNameAsync(string fileName);
        Task<int> GetAttachmentsCountByOrderAsync(int orderId);
        Task<long> GetTotalFileSizeByOrderAsync(int orderId);
    }
}