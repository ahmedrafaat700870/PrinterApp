using System.ComponentModel.DataAnnotations;

namespace PrinterApp.Models.ViewModels
{
    public class OrderDetailsViewModel
    {
        [Display(Name = "بيانات الطلب")]
        public OrderViewModel Order { get; set; }

        [Display(Name = "الملفات المرفقة")]
        public List<OrderAttachmentViewModel> Attachments { get; set; } = new List<OrderAttachmentViewModel>();

        [Display(Name = "عناصر التصنيع")]
        public List<ManufacturingItemViewModel> ManufacturingItems { get; set; } = new List<ManufacturingItemViewModel>();

        [Display(Name = "سجل الحركات")]
        public List<OrderTimelineViewModel> Timeline { get; set; } = new List<OrderTimelineViewModel>();

        // للحساب
        public int AttachmentsCount => Attachments?.Count ?? 0;
        public long TotalFileSize => Attachments?.Sum(a => a.FileSize) ?? 0;
        public string TotalFileSizeFormatted => FormatFileSize(TotalFileSize);

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
    }
}