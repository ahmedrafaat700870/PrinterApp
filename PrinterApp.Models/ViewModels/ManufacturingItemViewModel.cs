using System.ComponentModel.DataAnnotations;

namespace PrinterApp.Models.ViewModels
{
    public class ManufacturingItemViewModel
    {
        public int Id { get; set; }

        [Display(Name = "الإضافة التصنيعية")]
        public int ManufacturingAdditionId { get; set; }

        [Display(Name = "اسم الإضافة")]
        public string AdditionName { get; set; }

        [Display(Name = "مكتمل")]
        public bool IsCompleted { get; set; }

        [Display(Name = "تاريخ الإكمال")]
        public DateTime? CompletedDate { get; set; }

        [Display(Name = "أكمل بواسطة")]
        public string CompletedByName { get; set; }

        // للعرض
        public string StatusBadge => IsCompleted
            ? "<span class='badge bg-success'><i class='fas fa-check'></i> مكتمل</span>"
            : "<span class='badge bg-warning'><i class='fas fa-clock'></i> قيد التنفيذ</span>";
    }
}