using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrinterApp.Models.Entities
{
    public class OrderAttachment
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "الطلب")]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        [Required(ErrorMessage = "اسم الملف مطلوب")]
        [StringLength(255)]
        [Display(Name = "اسم الملف")]
        public string FileName { get; set; }

        [Required(ErrorMessage = "اسم الملف الأصلي مطلوب")]
        [StringLength(255)]
        [Display(Name = "اسم الملف الأصلي")]
        public string OriginalFileName { get; set; }

        [Required(ErrorMessage = "مسار الملف مطلوب")]
        [StringLength(500)]
        [Display(Name = "مسار الملف")]
        public string FilePath { get; set; }

        [Display(Name = "حجم الملف (بايت)")]
        public long FileSize { get; set; }

        [StringLength(50)]
        [Display(Name = "نوع الملف")]
        public string FileExtension { get; set; }

        [StringLength(100)]
        [Display(Name = "نوع المحتوى")]
        public string ContentType { get; set; }

        [Display(Name = "تاريخ الرفع")]
        public DateTime UploadedDate { get; set; } = DateTime.Now;

        [StringLength(450)]
        [Display(Name = "رفع بواسطة")]
        public string UploadedBy { get; set; }

        [Display(Name = "نشط")]
        public bool IsActive { get; set; } = true;
    }
}