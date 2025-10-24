using Microsoft.AspNetCore.Http;
using PrinterApp.Models.ViewModels;

namespace PrinterApp.Services.Interfaces
{
    public interface IOrderAttachmentService
    {
        // الحصول على المرفقات
        Task<IEnumerable<OrderAttachmentViewModel>> GetByOrderIdAsync(int orderId);
        Task<OrderAttachmentViewModel> GetByIdAsync(int id);
        Task<IEnumerable<OrderAttachmentViewModel>> GetActiveAttachmentsByOrderAsync(int orderId);

        // رفع الملفات
        Task<(bool Success, string[] Errors)> UploadFilesAsync(int orderId, IFormFileCollection files, string userId);
        Task<(bool Success, string FilePath, string ErrorMessage)> UploadSingleFileAsync(int orderId, IFormFile file, string userId);

        // حذف الملفات
        Task<(bool Success, string[] Errors)> DeleteFileAsync(int id);
        Task<(bool Success, string[] Errors)> DeleteMultipleFilesAsync(List<int> ids);

        // الإحصائيات
        Task<int> GetAttachmentsCountByOrderAsync(int orderId);
        Task<long> GetTotalFileSizeByOrderAsync(int orderId);
        Task<string> GetFormattedTotalFileSizeByOrderAsync(int orderId);

        // التحقق
        Task<bool> AttachmentExistsAsync(int id);
        Task<bool> AttachmentBelongsToOrderAsync(int attachmentId, int orderId);
    }
}