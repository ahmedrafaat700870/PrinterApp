using PrinterApp.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace PrinterApp.Models.ViewModels
{
    public class PrintQueueViewModel
    {
        public int Id { get; set; }

        [Display(Name = "الأولوية")]
        public int Priority { get; set; }

        [Display(Name = "رقم الطلب")]
        public string OrderNumber { get; set; }

        // Stage 1 - Order Info
        [Display(Name = "العميل")]
        public string CustomerName { get; set; }

        [Display(Name = "المورد")]
        public string SupplierName { get; set; }

        [Display(Name = "المنتج")]
        public string ProductName { get; set; }

        [Display(Name = "اتجاه الرول")]
        public string RollDirectionNumber { get; set; }

        [Display(Name = "صورة اتجاه الرول")]
        public string RollDirectionImage { get; set; }

        [Display(Name = "نوع الخام")]
        public string RawMaterialName { get; set; }

        [Display(Name = "الطول")]
        public decimal Length { get; set; }

        [Display(Name = "العرض")]
        public decimal Width { get; set; }

        [Display(Name = "الكمية")]
        public int Quantity { get; set; }

        // Stage 2 - Review Info
        [Display(Name = "الماكينة")]
        public string MachineName { get; set; }

        [Display(Name = "القلب")]
        public string CoreName { get; set; }

        [Display(Name = "السكينة")]
        public string KnifeName { get; set; }

        [Display(Name = "الكرتون")]
        public string CartonName { get; set; }

        [Display(Name = "القالب")]
        public string MoldNumber { get; set; }

        [Display(Name = "راجع بواسطة")]
        public string ReviewedBy { get; set; }

        // Stage 4 - Printing Info
        [Display(Name = "تاريخ بدء الطباعة")]
        public DateTime? PrintingStartDate { get; set; }

        [Display(Name = "طبع بواسطة")]
        public string PrintedBy { get; set; }

        // Dates
        [Display(Name = "تاريخ الطلب")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "تاريخ التسليم المتوقع")]
        public DateTime ExpectedDeliveryDate { get; set; }

        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "آخر تعديل")]
        public DateTime? LastModified { get; set; }

        // Status
        [Display(Name = "الحالة")]
        public OrderStatus Status { get; set; }

        [Display(Name = "المرحلة")]
        public OrderStage Stage { get; set; }

        [Display(Name = "نشط")]
        public bool IsActive { get; set; }

        [Display(Name = "متأخر")]
        public bool IsLate { get; set; }

        // Audit
        [Display(Name = "أنشئ بواسطة")]
        public string CreatedBy { get; set; }

        [Display(Name = "عدل بواسطة")]
        public string ModifiedBy { get; set; }

        // Counts
        public int AttachmentsCount { get; set; }
        public int ManufacturingItemsCount { get; set; }

        public string StatusText => Status.GetDisplayName();
        public string StageText => Stage.GetDisplayName();
        public string StatusBadgeClass => GetStatusBadgeClass();
        public string StageBadgeClass => GetStageBadgeClass();
        public string PriorityBadgeClass => GetPriorityBadgeClass();

        private string GetStatusBadgeClass()
        {
            return Status switch
            {
                OrderStatus.Pending => "badge bg-warning",
                OrderStatus.UnderReview => "badge bg-info",
                OrderStatus.InManufacturing => "badge bg-primary",
                OrderStatus.InPrinting => "badge bg-secondary",
                OrderStatus.Completed => "badge bg-success",
                OrderStatus.Cancelled => "badge bg-danger",
                OrderStatus.OnHold => "badge bg-dark",
                _ => "badge bg-secondary"
            };
        }

        private string GetStageBadgeClass()
        {
            return Stage switch
            {
                OrderStage.Order => "badge bg-warning",
                OrderStage.Review => "badge bg-info",
                OrderStage.Manufacturing => "badge bg-primary",
                OrderStage.Printing => "badge bg-secondary",
                OrderStage.Completed => "badge bg-success",
                _ => "badge bg-secondary"
            };
        }

        private string GetPriorityBadgeClass()
        {
            return Priority switch
            {
                <= 5 => "badge bg-danger",
                <= 10 => "badge bg-warning",
                <= 20 => "badge bg-info",
                _ => "badge bg-secondary"
            };
        }
    }
}
