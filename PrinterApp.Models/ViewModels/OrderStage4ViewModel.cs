using System.ComponentModel.DataAnnotations;

namespace PrinterApp.Models.ViewModels
{
    public class OrderStage4ViewModel
    {
        public int Id { get; set; }

        // المعلومات الأساسية
        [Display(Name = "رقم الطلب")]
        public string OrderNumber { get; set; }

        [Display(Name = "تاريخ الطلب")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "تاريخ التسليم المتوقع")]
        public DateTime ExpectedDeliveryDate { get; set; }

        // معلومات العميل والمنتج
        [Display(Name = "اسم العميل")]
        public string CustomerName { get; set; }

        [Display(Name = "هاتف العميل")]
        public string CustomerPhone { get; set; }

        [Display(Name = "المنتج")]
        public string ProductName { get; set; }

        [Display(Name = "المورد")]
        public string SupplierName { get; set; }

        // المواصفات
        [Display(Name = "الطول (سم)")]
        public decimal Length { get; set; }

        [Display(Name = "العرض (سم)")]
        public decimal Width { get; set; }

        [Display(Name = "الكمية")]
        public int Quantity { get; set; }

        [Display(Name = "نوع الخام")]
        public string RawMaterialName { get; set; }

        [Display(Name = "اتجاه الرول")]
        public string RollDirectionNumber { get; set; }

        [Display(Name = "صورة اتجاه الرول")]
        public string RollDirectionImage { get; set; }

        // معلومات المراجعة
        [Display(Name = "الماكينة")]
        public string MachineName { get; set; }

        [Display(Name = "القلب (Core)")]
        public string CoreName { get; set; }

        [Display(Name = "السكينة")]
        public string KnifeName { get; set; }

        [Display(Name = "الكرتون")]
        public string CartonName { get; set; }

        [Display(Name = "القالب")]
        public string MoldName { get; set; }

        // الملاحظات
        [Display(Name = "ملاحظات الطلب")]
        public string OrderNotes { get; set; }

        [Display(Name = "ملاحظات المراجعة")]
        public string ReviewNotes { get; set; }

        [Display(Name = "ملاحظات التصنيع")]
        public string ManufacturingNotes { get; set; }

        [StringLength(2000)]
        [Display(Name = "ملاحظات الطباعة")]
        public string PrintingNotes { get; set; }

        // حالة الطباعة
        [Display(Name = "تاريخ بدء الطباعة")]
        public DateTime? PrintingStartDate { get; set; }

        [Display(Name = "تاريخ انتهاء الطباعة")]
        public DateTime? PrintingEndDate { get; set; }

        [Display(Name = "طبع بواسطة")]
        public string PrintedBy { get; set; }

        // للتحكم
        public bool CanStartPrinting => !PrintingStartDate.HasValue;
        public bool CanCompletePrinting => PrintingStartDate.HasValue && !PrintingEndDate.HasValue;
    }
}