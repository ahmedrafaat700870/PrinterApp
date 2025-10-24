using System.ComponentModel.DataAnnotations;

namespace PrinterApp.Models.ViewModels
{
    public class OrderStage3ViewModel
    {
        public int Id { get; set; }

        // معلومات الطلب (للعرض فقط)
        [Display(Name = "رقم الطلب")]
        public string OrderNumber { get; set; }

        [Display(Name = "العميل")]
        public string CustomerName { get; set; }

        [Display(Name = "المنتج")]
        public string ProductName { get; set; }

        [Display(Name = "الكمية")]
        public int Quantity { get; set; }

        // عناصر التصنيع
        [Display(Name = "عناصر التصنيع")]
        public List<ManufacturingItemViewModel> ManufacturingItems { get; set; } = new List<ManufacturingItemViewModel>();

        [StringLength(2000)]
        [Display(Name = "ملاحظات التصنيع")]
        public string ManufacturingNotes { get; set; }

        // للحساب
        public int TotalItems => ManufacturingItems?.Count ?? 0;
        public int CompletedItems => ManufacturingItems?.Count(mi => mi.IsCompleted) ?? 0;
        public double CompletionPercentage => TotalItems > 0 ? (double)CompletedItems / TotalItems * 100 : 0;
        public bool AllItemsCompleted => TotalItems > 0 && CompletedItems == TotalItems;
    }
}