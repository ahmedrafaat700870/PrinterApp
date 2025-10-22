using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace PrinterApp.Web.Helpers
{
    public static class FileUploadHelper
    {
        public static async Task<string> SaveImageAsync(IFormFile imageFile, IWebHostEnvironment webHostEnvironment)
        {
            if (imageFile == null || imageFile.Length == 0)
                return null;

            try
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploads", "directions");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                return $"/uploads/directions/{uniqueFileName}";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving image: {ex.Message}");
            }
        }

        

        /// <summary>
        /// Upload file with validation
        /// </summary>
        /// <param name="file">The file to upload</param>
        /// <param name="webRootPath">Web root path from IWebHostEnvironment</param>
        /// <param name="uploadFolder">Relative upload folder path (e.g., "uploads/mold-shapes")</param>
        /// <param name="allowedExtensions">Allowed file extensions (e.g., new[] { ".jpg", ".png" })</param>
        /// <param name="maxSizeInBytes">Maximum file size in bytes (default: 5MB)</param>
        /// <returns>FileUploadResult with success status and file path or error message</returns>
        public static async Task<FileUploadResult> UploadFileAsync(
            IFormFile file,
            string webRootPath,
            string uploadFolder,
            string[] allowedExtensions = null,
            long maxSizeInBytes = 5242880) // 5MB default
        {
            if (file == null || file.Length == 0)
            {
                return new FileUploadResult
                {
                    Success = false,
                    ErrorMessage = "No file selected"
                };
            }

            try
            {
                // Validate file extension
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (allowedExtensions != null && allowedExtensions.Length > 0)
                {
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        return new FileUploadResult
                        {
                            Success = false,
                            ErrorMessage = $"Invalid file type. Allowed types: {string.Join(", ", allowedExtensions)}"
                        };
                    }
                }

                // Validate file size
                if (file.Length > maxSizeInBytes)
                {
                    var maxSizeInMB = maxSizeInBytes / 1024.0 / 1024.0;
                    return new FileUploadResult
                    {
                        Success = false,
                        ErrorMessage = $"File size exceeds the maximum allowed size of {maxSizeInMB:F2} MB"
                    };
                }

                // Create upload directory if it doesn't exist
                var fullUploadPath = Path.Combine(webRootPath, uploadFolder.Replace("/", Path.DirectorySeparatorChar.ToString()));

                if (!Directory.Exists(fullUploadPath))
                {
                    Directory.CreateDirectory(fullUploadPath);
                }

                // Generate unique filename
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(fullUploadPath, uniqueFileName);

                // Save file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Return relative path with forward slashes
                var relativePath = $"/{uploadFolder}/{uniqueFileName}".Replace("\\", "/");

                return new FileUploadResult
                {
                    Success = true,
                    FilePath = relativePath
                };
            }
            catch (Exception ex)
            {
                return new FileUploadResult
                {
                    Success = false,
                    ErrorMessage = $"Error uploading file: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Save image file (legacy method for backward compatibility)
        /// </summary>
        public static async Task<string> SaveImageAsync(IFormFile imageFile, IWebHostEnvironment webHostEnvironment, string folder = "directions")
        {
            if (imageFile == null || imageFile.Length == 0)
                return null;

            try
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploads", folder);

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                return $"/uploads/{folder}/{uniqueFileName}";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving image: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete file from server
        /// </summary>
        /// <param name="webRootPath">Web root path from IWebHostEnvironment</param>
        /// <param name="filePath">Relative file path (e.g., "/uploads/mold-shapes/image.jpg")</param>
        public static void DeleteFile(string webRootPath, string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return;

            try
            {
                var fullPath = Path.Combine(webRootPath, filePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            catch
            {
                // Ignore errors - file might not exist or be in use
            }
        }

        /// <summary>
        /// Delete image async (legacy method for backward compatibility)
        /// </summary>
        public static async Task DeleteImageAsync(string imagePath, IWebHostEnvironment webHostEnvironment)
        {
            if (string.IsNullOrEmpty(imagePath))
                return;

            try
            {
                string fullPath = Path.Combine(webHostEnvironment.WebRootPath, imagePath.TrimStart('/'));

                if (File.Exists(fullPath))
                {
                    await Task.Run(() => File.Delete(fullPath));
                }
            }
            catch
            {
                // Ignore errors
            }
        }

        /// <summary>
        /// Get file size in human-readable format
        /// </summary>
        public static string GetFileSizeString(long bytes)
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

        /// <summary>
        /// Validate image file
        /// </summary>
        public static bool IsValidImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            return allowedExtensions.Contains(extension);
        }

        /// <summary>
        /// Get content type from file extension
        /// </summary>
        public static string GetContentType(string fileName)
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
                _ => "application/octet-stream"
            };
        }
    }


    public class FileUploadResult
    {
        public bool Success { get; set; }
        public string FilePath { get; set; }
        public string ErrorMessage { get; set; }
    }
}