using PrinterApp.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace PrinterApp.Models.ViewModels
{
    public class OrderViewModel
    {
        public int Id { get; set; }

        [Display(Name = "رقم الطلب")]
        public string OrderNumber { get; set; }

        [Display(Name = "تاريخ الطلب")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "تاريخ التسليم المتوقع")]
        public DateTime ExpectedDeliveryDate { get; set; }

        [Display(Name = "تاريخ التسليم الفعلي")]
        public DateTime? ActualDeliveryDate { get; set; }

        // ✅ إضافة الـ IDs المطلوبة للـ Edit
        public int CustomerId { get; set; }
        public int SupplierId { get; set; }
        public int ProductId { get; set; }
        public int? RollDirectionId { get; set; }
        public int? RawMaterialId { get; set; }

        [Display(Name = "العميل")]
        public string CustomerName { get; set; }

        [Display(Name = "المورد")]
        public string SupplierName { get; set; }

        [Display(Name = "المنتج")]
        public string ProductName { get; set; }

        [Display(Name = "الكمية")]
        public int Quantity { get; set; }

        [Display(Name = "الطول")]
        public decimal Length { get; set; }

        [Display(Name = "العرض")]
        public decimal Width { get; set; }

        [Display(Name = "الحالة")]
        public OrderStatus Status { get; set; }

        [Display(Name = "المرحلة")]
        public OrderStage Stage { get; set; }

        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "آخر تعديل")]
        public DateTime? LastModified { get; set; }

        [Display(Name = "نشط")]
        public bool IsActive { get; set; }

        [Display(Name = "متأخر")]
        public bool IsLate { get; set; }

        // ✅ إضافة OrderNotes
        [Display(Name = "ملاحظات الطلب")]
        public string? OrderNotes { get; set; }

        // للعرض
        public string StatusText => Status.GetDisplayName();
        public string StageText => Stage.GetDisplayName();
        public string StatusBadgeClass => GetStatusBadgeClass();
        public string StageBadgeClass => GetStageBadgeClass();

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
    }
}