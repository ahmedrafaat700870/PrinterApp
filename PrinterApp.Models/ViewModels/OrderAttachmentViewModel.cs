namespace PrinterApp.Models.ViewModels
{
    public class OrderAttachmentViewModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public string FileExtension { get; set; }
        public string ContentType { get; set; }
        public DateTime UploadedDate { get; set; }
        public string UploadedBy { get; set; }
        public bool IsActive { get; set; }

        // Computed Properties
        public string FormattedFileSize => FormatFileSize(FileSize);
        public string FileIcon => GetFileIcon();
        public bool IsImage => IsImageFile();
        public bool IsPdf => FileExtension?.ToLower() == ".pdf";
        public bool IsDocument => IsDocumentFile();

        private string FormatFileSize(long bytes)
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

        private string GetFileIcon()
        {
            return FileExtension?.ToLower() switch
            {
                ".pdf" => "bi-file-pdf-fill text-danger",
                ".doc" or ".docx" => "bi-file-word-fill text-primary",
                ".xls" or ".xlsx" => "bi-file-excel-fill text-success",
                ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp" => "bi-file-image-fill text-info",
                _ => "bi-file-earmark-fill text-secondary"
            };
        }

        private bool IsImageFile()
        {
            var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            return imageExtensions.Contains(FileExtension?.ToLower());
        }

        private bool IsDocumentFile()
        {
            var docExtensions = new[] { ".doc", ".docx", ".xls", ".xlsx", ".pdf" };
            return docExtensions.Contains(FileExtension?.ToLower());
        }
    }
}