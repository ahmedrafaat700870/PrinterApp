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
    }
}