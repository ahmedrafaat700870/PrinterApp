using System.ComponentModel.DataAnnotations;

namespace PrinterApp.Models.ViewModels
{
    public class OrderStage2ViewModel
    {
        public int Id { get; set; }

        // معلومات الطلب (للعرض فقط)
        [Display(Name = "رقم الطلب")]
        public string OrderNumber { get; set; }

        [Display(Name = "تاريخ الطلب")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "العميل")]
        public string CustomerName { get; set; }

        [Display(Name = "المنتج")]
        public string ProductName { get; set; }

        [Display(Name = "الكمية")]
        public int Quantity { get; set; }

        [Display(Name = "الطول")]
        public decimal Length { get; set; }

        [Display(Name = "العرض")]
        public decimal Width { get; set; }

        [Display(Name = "نوع الخام")]
        public string RawMaterialName { get; set; }

        // بيانات المراجعة
        [Required(ErrorMessage = "الماكينة مطلوبة")]
        [Display(Name = "الماكينة")]
        public int? MachineId { get; set; }

        [Required(ErrorMessage = "القلب (Core) مطلوب")]
        [Display(Name = "القلب (Core)")]
        public int? CoreId { get; set; }

        [Required(ErrorMessage = "السكينة مطلوبة")]
        [Display(Name = "السكينة")]
        public int? KnifeId { get; set; }

        [Required(ErrorMessage = "الكرتون مطلوب")]
        [Display(Name = "الكرتون")]
        public int? CartonId { get; set; }

        [Display(Name = "القالب")]
        public int? MoldId { get; set; }

        [StringLength(2000)]
        [Display(Name = "ملاحظات المراجعة")]
        public string ReviewNotes { get; set; }

        // الإضافات التصنيعية
        [Display(Name = "الإضافات التصنيعية")]
        public List<int> SelectedManufacturingAdditions { get; set; } = new List<int>();

        // للعرض في الـ Form
        public List<MachineViewModel> Machines { get; set; }
        public List<CoreViewModel> Cores { get; set; }
        public List<KnifeViewModel> Knives { get; set; }
        public List<CartonViewModel> Cartons { get; set; }
        public List<MoldViewModel> Molds { get; set; }
        public List<ManufacturingAdditionViewModel> ManufacturingAdditions { get; set; }
    }
}