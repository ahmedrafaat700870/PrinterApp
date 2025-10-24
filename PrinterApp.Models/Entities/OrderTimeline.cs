using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrinterApp.Models.Entities
{
    public class OrderTimeline
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "الطلب")]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        [Required]
        [Display(Name = "المرحلة")]
        public OrderStage Stage { get; set; }

        [Required]
        [Display(Name = "الحالة")]
        public OrderStatus Status { get; set; }

        [Required(ErrorMessage = "الإجراء مطلوب")]
        [StringLength(200)]
        [Display(Name = "الإجراء")]
        public string Action { get; set; }

        [StringLength(1000)]
        [Display(Name = "ملاحظات")]
        public string Notes { get; set; }

        [Display(Name = "تاريخ الإجراء")]
        public DateTime ActionDate { get; set; } = DateTime.Now;

        [StringLength(450)]
        [Display(Name = "بواسطة")]
        public string ActionBy { get; set; }

        [StringLength(100)]
        [Display(Name = "اسم المستخدم")]
        public string ActionByName { get; set; }
    }
}