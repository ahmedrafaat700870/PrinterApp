using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrinterApp.Models.Entities
{
    public class OrderManufacturingItem
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "الطلب")]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        [Required]
        [Display(Name = "الإضافة التصنيعية")]
        public int ManufacturingAdditionId { get; set; }

        [ForeignKey("ManufacturingAdditionId")]
        public virtual ManufacturingAddition ManufacturingAddition { get; set; }

        [Display(Name = "مكتمل")]
        public bool IsCompleted { get; set; } = false;

        [Display(Name = "تاريخ الإكمال")]
        public DateTime? CompletedDate { get; set; }

        [StringLength(450)]
        [Display(Name = "أكمل بواسطة")]
        public string CompletedBy { get; set; }

        [StringLength(500)]
        [Display(Name = "ملاحظات")]
        public string Notes { get; set; }

        [Display(Name = "الترتيب")]
        public int DisplayOrder { get; set; }
    }
}