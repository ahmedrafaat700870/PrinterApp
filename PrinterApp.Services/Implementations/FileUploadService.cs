using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using PrinterApp.Services.Interfaces;

namespace PrinterApp.Services.Implementations
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IHostEnvironment _hostEnvironment;
        private readonly string _webRootPath;

        public FileUploadService(IHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
            // تحديد مسار wwwroot يدوياً
            _webRootPath = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot");

            // التأكد من وجود مجلد wwwroot
            if (!Directory.Exists(_webRootPath))
            {
                Directory.CreateDirectory(_webRootPath);
            }
        }

        // ===== رفع ملف واحد =====
        public async Task<(bool Success, string FilePath, string ErrorMessage)> UploadFileAsync(
            IFormFile file,
            string folderPath,
            string[] allowedExtensions = null,
            long maxSizeInBytes = 10485760) // 10MB default
        {
            if (file == null || file.Length == 0)
            {
                return (false, null, "لم يتم اختيار ملف");
            }

            try
            {
                // التحقق من نوع الملف
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (allowedExtensions != null && allowedExtensions.Length > 0)
                {
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        return (false, null, $"نوع الملف غير مسموح. الأنواع المسموحة: {string.Join(", ", allowedExtensions)}");
                    }
                }

                // التحقق من حجم الملف
                if (file.Length > maxSizeInBytes)
                {
                    var maxSizeInMB = maxSizeInBytes / 1024.0 / 1024.0;
                    return (false, null, $"حجم الملف يتجاوز الحد المسموح ({maxSizeInMB:F2} MB)");
                }

                // إنشاء مسار التحميل الكامل
                var fullUploadPath = Path.Combine(_webRootPath, folderPath.Replace("/", Path.DirectorySeparatorChar.ToString()));

                // إنشاء المجلد إذا لم يكن موجوداً
                if (!Directory.Exists(fullUploadPath))
                {
                    Directory.CreateDirectory(fullUploadPath);
                }

                // إنشاء اسم فريد للملف
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var fullFilePath = Path.Combine(fullUploadPath, uniqueFileName);

                // حفظ الملف
                using (var fileStream = new FileStream(fullFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // إرجاع المسار النسبي
                var relativePath = $"/{folderPath}/{uniqueFileName}".Replace("\\", "/");

                return (true, relativePath, null);
            }
            catch (Exception ex)
            {
                return (false, null, $"خطأ أثناء رفع الملف: {ex.Message}");
            }
        }

        // ===== رفع ملفات متعددة =====
        public async Task<(bool Success, List<string> FilePaths, List<string> Errors)> UploadFilesAsync(
            IFormFileCollection files,
            string folderPath,
            string[] allowedExtensions = null,
            long maxSizeInBytes = 10485760)
        {
            var uploadedFiles = new List<string>();
            var errors = new List<string>();

            if (files == null || files.Count == 0)
            {
                errors.Add("لم يتم اختيار أي ملفات");
                return (false, uploadedFiles, errors);
            }

            foreach (var file in files)
            {
                var result = await UploadFileAsync(file, folderPath, allowedExtensions, maxSizeInBytes);

                if (result.Success)
                {
                    uploadedFiles.Add(result.FilePath);
                }
                else
                {
                    errors.Add($"{file.FileName}: {result.ErrorMessage}");
                }
            }

            return (uploadedFiles.Any(), uploadedFiles, errors);
        }

        // ===== حذف ملف =====
        public async Task<bool> DeleteFileAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return false;

            try
            {
                // تحويل المسار النسبي إلى مسار كامل
                var fullPath = Path.Combine(_webRootPath, filePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

                if (File.Exists(fullPath))
                {
                    await Task.Run(() => File.Delete(fullPath));
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        // ===== التحقق من وجود ملف =====
        public bool FileExists(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return false;

            try
            {
                var fullPath = Path.Combine(_webRootPath, filePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                return File.Exists(fullPath);
            }
            catch
            {
                return false;
            }
        }

        // ===== الحصول على حجم الملف =====
        public long GetFileSize(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return 0;

            try
            {
                var fullPath = Path.Combine(_webRootPath, filePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

                if (File.Exists(fullPath))
                {
                    var fileInfo = new FileInfo(fullPath);
                    return fileInfo.Length;
                }

                return 0;
            }
            catch
            {
                return 0;
            }
        }

        // ===== تنسيق حجم الملف =====
        public string GetFileSizeFormatted(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return $"{len:0.##} {sizes[order]}";
        }

        // ===== الحصول على Content Type =====
        public string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".zip" => "application/zip",
                ".rar" => "application/x-rar-compressed",
                ".txt" => "text/plain",
                _ => "application/octet-stream"
            };
        }
    }
}