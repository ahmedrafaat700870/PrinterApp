using Microsoft.AspNetCore.Http;
using PrinterApp.Data.UnitOfWork;
using PrinterApp.Models.Entities;
using PrinterApp.Models.ViewModels;
using PrinterApp.Services.Interfaces;

namespace PrinterApp.Services.Implementations
{
    public class OrderAttachmentService : IOrderAttachmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileUploadService _fileUploadService;
        private readonly string[] _allowedExtensions = { ".pdf", ".jpg", ".jpeg", ".png", ".gif", ".doc", ".docx", ".xls", ".xlsx", ".bmp" };
        private readonly long _maxFileSize = 10 * 1024 * 1024; // 10 MB

        public OrderAttachmentService(IUnitOfWork unitOfWork, IFileUploadService fileUploadService)
        {
            _unitOfWork = unitOfWork;
            _fileUploadService = fileUploadService;
        }

        // ===== الحصول على المرفقات =====

        public async Task<IEnumerable<OrderAttachmentViewModel>> GetByOrderIdAsync(int orderId)
        {
            var attachments = await _unitOfWork.OrderAttachments.GetByOrderIdAsync(orderId);
            return attachments.Select(MapToViewModel);
        }

        public async Task<OrderAttachmentViewModel> GetByIdAsync(int id)
        {
            var attachment = await _unitOfWork.OrderAttachments.GetByIdAsync(id);

            if (attachment == null)
                return null;

            return MapToViewModel(attachment);
        }

        public async Task<IEnumerable<OrderAttachmentViewModel>> GetActiveAttachmentsByOrderAsync(int orderId)
        {
            var attachments = await _unitOfWork.OrderAttachments.GetByOrderIdAsync(orderId);
            return attachments.Where(a => a.IsActive).Select(MapToViewModel);
        }

        // ===== رفع الملفات =====

        public async Task<(bool Success, string[] Errors)> UploadFilesAsync(int orderId, IFormFileCollection files, string userId)
        {
            var errors = new List<string>();

            if (files == null || files.Count == 0)
            {
                errors.Add("لم يتم اختيار أي ملفات");
                return (false, errors.ToArray());
            }

            // التحقق من وجود الطلب
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
            {
                errors.Add("الطلب غير موجود");
                return (false, errors.ToArray());
            }

            var folderPath = $"uploads/orders/{orderId}";

            try
            {
                var uploadResult = await _fileUploadService.UploadFilesAsync(files, folderPath, _allowedExtensions, _maxFileSize);

                if (uploadResult.Errors.Any())
                {
                    errors.AddRange(uploadResult.Errors);
                }

                // حفظ معلومات الملفات في قاعدة البيانات
                for (int i = 0; i < uploadResult.FilePaths.Count; i++)
                {
                    var filePath = uploadResult.FilePaths[i];

                    // العثور على الملف المقابل
                    var file = files.FirstOrDefault(f =>
                        uploadResult.FilePaths.IndexOf(filePath) == files.ToList().IndexOf(f));

                    if (file != null)
                    {
                        var attachment = new OrderAttachment
                        {
                            OrderId = orderId,
                            FileName = Path.GetFileName(filePath),
                            OriginalFileName = file.FileName,
                            FilePath = filePath,
                            FileSize = file.Length,
                            FileExtension = Path.GetExtension(file.FileName).ToLowerInvariant(),
                            ContentType = _fileUploadService.GetContentType(file.FileName),
                            UploadedDate = DateTime.Now,
                            UploadedBy = userId,
                            IsActive = true
                        };

                        await _unitOfWork.OrderAttachments.AddAsync(attachment);
                    }
                }

                await _unitOfWork.CompleteAsync();

                return (uploadResult.Success && !errors.Any(), errors.ToArray());
            }
            catch (Exception ex)
            {
                errors.Add($"خطأ أثناء رفع الملفات: {ex.Message}");
                return (false, errors.ToArray());
            }
        }

        public async Task<(bool Success, string FilePath, string ErrorMessage)> UploadSingleFileAsync(int orderId, IFormFile file, string userId)
        {
            if (file == null || file.Length == 0)
            {
                return (false, null, "لم يتم اختيار ملف");
            }

            // التحقق من وجود الطلب
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
            {
                return (false, null, "الطلب غير موجود");
            }

            var folderPath = $"uploads/orders/{orderId}";

            try
            {
                var uploadResult = await _fileUploadService.UploadFileAsync(file, folderPath, _allowedExtensions, _maxFileSize);

                if (!uploadResult.Success)
                {
                    return (false, null, uploadResult.ErrorMessage);
                }

                // حفظ معلومات الملف في قاعدة البيانات
                var attachment = new OrderAttachment
                {
                    OrderId = orderId,
                    FileName = Path.GetFileName(uploadResult.FilePath),
                    OriginalFileName = file.FileName,
                    FilePath = uploadResult.FilePath,
                    FileSize = file.Length,
                    FileExtension = Path.GetExtension(file.FileName).ToLowerInvariant(),
                    ContentType = _fileUploadService.GetContentType(file.FileName),
                    UploadedDate = DateTime.Now,
                    UploadedBy = userId,
                    IsActive = true
                };

                await _unitOfWork.OrderAttachments.AddAsync(attachment);
                await _unitOfWork.CompleteAsync();

                return (true, uploadResult.FilePath, null);
            }
            catch (Exception ex)
            {
                return (false, null, $"خطأ أثناء رفع الملف: {ex.Message}");
            }
        }

        // ===== حذف الملفات =====

        public async Task<(bool Success, string[] Errors)> DeleteFileAsync(int id)
        {
            var errors = new List<string>();

            try
            {
                var attachment = await _unitOfWork.OrderAttachments.GetByIdAsync(id);

                if (attachment == null)
                {
                    errors.Add("المرفق غير موجود");
                    return (false, errors.ToArray());
                }

                // حذف الملف الفعلي من السيرفر
                var fileDeleted = await _fileUploadService.DeleteFileAsync(attachment.FilePath);

                if (!fileDeleted)
                {
                    errors.Add("فشل في حذف الملف من السيرفر");
                }

                // حذف السجل من قاعدة البيانات (Soft Delete)
                attachment.IsActive = false;
                _unitOfWork.OrderAttachments.Update(attachment);
                await _unitOfWork.CompleteAsync();

                return (true, errors.ToArray());
            }
            catch (Exception ex)
            {
                errors.Add($"خطأ أثناء حذف الملف: {ex.Message}");
                return (false, errors.ToArray());
            }
        }

        public async Task<(bool Success, string[] Errors)> DeleteMultipleFilesAsync(List<int> ids)
        {
            var errors = new List<string>();
            var successCount = 0;

            foreach (var id in ids)
            {
                var result = await DeleteFileAsync(id);

                if (result.Success)
                {
                    successCount++;
                }
                else
                {
                    errors.AddRange(result.Errors);
                }
            }

            return (successCount > 0, errors.ToArray());
        }

        // ===== الإحصائيات =====

        public async Task<int> GetAttachmentsCountByOrderAsync(int orderId)
        {
            return await _unitOfWork.OrderAttachments.GetAttachmentsCountByOrderAsync(orderId);
        }

        public async Task<long> GetTotalFileSizeByOrderAsync(int orderId)
        {
            return await _unitOfWork.OrderAttachments.GetTotalFileSizeByOrderAsync(orderId);
        }

        public async Task<string> GetFormattedTotalFileSizeByOrderAsync(int orderId)
        {
            var totalSize = await GetTotalFileSizeByOrderAsync(orderId);
            return _fileUploadService.GetFileSizeFormatted(totalSize);
        }

        // ===== التحقق =====

        public async Task<bool> AttachmentExistsAsync(int id)
        {
            var attachment = await _unitOfWork.OrderAttachments.GetByIdAsync(id);
            return attachment != null && attachment.IsActive;
        }

        public async Task<bool> AttachmentBelongsToOrderAsync(int attachmentId, int orderId)
        {
            var attachment = await _unitOfWork.OrderAttachments.GetByIdAsync(attachmentId);
            return attachment != null && attachment.OrderId == orderId;
        }

        // ===== Helper Method =====

        private OrderAttachmentViewModel MapToViewModel(OrderAttachment attachment)
        {
            return new OrderAttachmentViewModel
            {
                Id = attachment.Id,
                OrderId = attachment.OrderId,
                FileName = attachment.FileName,
                OriginalFileName = attachment.OriginalFileName,
                FilePath = attachment.FilePath,
                FileSize = attachment.FileSize,
                FileExtension = attachment.FileExtension,
                ContentType = attachment.ContentType,
                UploadedDate = attachment.UploadedDate,
                UploadedBy = attachment.UploadedBy,
                IsActive = attachment.IsActive
            };
        }
    }
}