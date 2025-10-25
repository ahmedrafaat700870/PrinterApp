using Microsoft.AspNetCore.Mvc.Rendering;
using PrinterApp.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace PrinterApp.Models.ViewModels
{
    public class PrintOrderViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "رقم الطلب مطلوب")]
        [StringLength(50, ErrorMessage = "رقم الطلب لا يمكن أن يتجاوز 50 حرف")]
        [Display(Name = "رقم الطلب")]
        public string OrderNumber { get; set; }

        [Required(ErrorMessage = "تاريخ الطلب مطلوب")]
        [Display(Name = "تاريخ الطلب")]
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "تاريخ التسليم المتوقع مطلوب")]
        [Display(Name = "تاريخ التسليم المتوقع")]
        [DataType(DataType.Date)]
        public DateTime ExpectedDeliveryDate { get; set; }

        [Display(Name = "تاريخ التسليم الفعلي")]
        [DataType(DataType.Date)]
        public DateTime? ActualDeliveryDate { get; set; }

        [Required(ErrorMessage = "المورد مطلوب")]
        [Display(Name = "المورد")]
        public int SupplierId { get; set; }

        [Display(Name = "اسم المورد")]
        public string SupplierName { get; set; }

        [Required(ErrorMessage = "المنتج مطلوب")]
        [Display(Name = "المنتج")]
        public int ProductId { get; set; }

        [Display(Name = "اسم المنتج")]
        public string ProductName { get; set; }

        [Display(Name = "اتجاه الرول")]
        public int? RollDirectionId { get; set; }

        [Display(Name = "رقم اتجاه الرول")]
        public int? RollDirectionNumber { get; set; }

        [Required(ErrorMessage = "الكمية مطلوبة")]
        [Range(1, int.MaxValue, ErrorMessage = "الكمية يجب أن تكون على الأقل 1")]
        [Display(Name = "الكمية")]
        public int Quantity { get; set; }

        [StringLength(1000)]
        [Display(Name = "ملاحظات")]
        public string Notes { get; set; }

        [Required]
        [Display(Name = "حالة الطلب")]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [Required]
        [Display(Name = "مرحلة الطلب")]
        public OrderStage Stage { get; set; } = OrderStage.Order;

        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "آخر تعديل")]
        public DateTime? LastModified { get; set; }

        [Display(Name = "نشط")]
        public bool IsActive { get; set; } = true;

        [Range(1, int.MaxValue, ErrorMessage = "الأولوية يجب أن تكون رقم موجب")]
        [Display(Name = "أولوية الطباعة")]
        public int Priority { get; set; } = 999;

        // قوائم منسدلة
        public SelectList Suppliers { get; set; }
        public SelectList Products { get; set; }
        public SelectList RollDirections { get; set; }
    }
}