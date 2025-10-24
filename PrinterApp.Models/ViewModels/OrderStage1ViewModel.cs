using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace PrinterApp.Models.ViewModels
{
    public class OrderStage1ViewModel
    {
        public int Id { get; set; }

        // المعلومات الأساسية
        [Required(ErrorMessage = "رقم الطلب مطلوب")]
        [StringLength(50)]
        [Display(Name = "رقم الطلب")]
        public string OrderNumber { get; set; }

        [Required(ErrorMessage = "تاريخ الطلب مطلوب")]
        [Display(Name = "تاريخ الطلب")]
        public DateTime OrderDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "تاريخ التسليم المتوقع مطلوب")]
        [Display(Name = "تاريخ التسليم المتوقع")]
        public DateTime ExpectedDeliveryDate { get; set; }

        // معلومات العميل والصنف
        [Required(ErrorMessage = "العميل مطلوب")]
        [Display(Name = "العميل")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "المورد مطلوب")]
        [Display(Name = "المورد")]
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "المنتج مطلوب")]
        [Display(Name = "المنتج")]
        public int ProductId { get; set; }

        // مواصفات المنتج
        [Display(Name = "اتجاه الرول")]
        public int? RollDirectionId { get; set; }

        [Required(ErrorMessage = "نوع الخام مطلوب")]
        [Display(Name = "نوع الخام")]
        public int RawMaterialId { get; set; }

        [Required(ErrorMessage = "الطول مطلوب")]
        [Range(0.01, double.MaxValue, ErrorMessage = "الطول يجب أن يكون أكبر من صفر")]
        [Display(Name = "الطول (سم)")]
        public decimal Length { get; set; }

        [Required(ErrorMessage = "العرض مطلوب")]
        [Range(0.01, double.MaxValue, ErrorMessage = "العرض يجب أن يكون أكبر من صفر")]
        [Display(Name = "العرض (سم)")]
        public decimal Width { get; set; }

        [Required(ErrorMessage = "الكمية مطلوبة")]
        [Range(1, int.MaxValue, ErrorMessage = "الكمية يجب أن تكون على الأقل 1")]
        [Display(Name = "الكمية")]
        public int Quantity { get; set; }

        [StringLength(2000)]
        [Display(Name = "ملاحظات الطلب")]
        public string OrderNotes { get; set; }

        // رفع ملفات
        [Display(Name = "الملفات المرفقة")]
        public IFormFileCollection Files { get; set; }

        // للعرض في الـ Form
        public List<CustomerViewModel> Customers { get; set; } = new List<CustomerViewModel>();
        public List<SupplierViewModel> Suppliers { get; set; } = new List<SupplierViewModel>();
        public List<ProductViewModel> Products { get; set; } = new List<ProductViewModel>();
        public List<RollDirectionViewModel> RollDirections { get; set; } = new List<RollDirectionViewModel>();
        public List<RawMaterialViewModel> RawMaterials { get; set; } = new List<RawMaterialViewModel>();
    }
}