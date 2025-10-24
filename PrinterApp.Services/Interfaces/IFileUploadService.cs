using Microsoft.AspNetCore.Http;

namespace PrinterApp.Services.Interfaces
{
    public interface IFileUploadService
    {
        Task<(bool Success, string FilePath, string ErrorMessage)> UploadFileAsync(
            IFormFile file,
            string folderPath,
            string[] allowedExtensions = null,
            long maxSizeInBytes = 10485760);

        Task<(bool Success, List<string> FilePaths, List<string> Errors)> UploadFilesAsync(
            IFormFileCollection files,
            string folderPath,
            string[] allowedExtensions = null,
            long maxSizeInBytes = 10485760);

        Task<bool> DeleteFileAsync(string filePath);

        bool FileExists(string filePath);

        long GetFileSize(string filePath);

        string GetFileSizeFormatted(long bytes);

        string GetContentType(string fileName);
    }
}