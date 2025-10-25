using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrinterApp.Models.Entities
{
    public class Order
    {
        public int Id { get; set; }

        // ===== معلومات أساسية =====
        [Required(ErrorMessage = "رقم الطلب مطلوب")]
        [StringLength(50, ErrorMessage = "رقم الطلب لا يمكن أن يتجاوز 50 حرف")]
        [Display(Name = "رقم الطلب")]
        public string OrderNumber { get; set; }

        [Required(ErrorMessage = "تاريخ الطلب مطلوب")]
        [Display(Name = "تاريخ الطلب")]
        public DateTime OrderDate { get; set; }

        [Required(ErrorMessage = "تاريخ التسليم المتوقع مطلوب")]
        [Display(Name = "تاريخ التسليم المتوقع")]
        public DateTime ExpectedDeliveryDate { get; set; }

        [Display(Name = "تاريخ التسليم الفعلي")]
        public DateTime? ActualDeliveryDate { get; set; }

        // ===== مرحلة الطلب (Stage 1) =====

        // معلومات العميل والصنف
        [Required(ErrorMessage = "العميل مطلوب")]
        [Display(Name = "العميل")]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        [Required(ErrorMessage = "المورد مطلوب")]
        [Display(Name = "المورد")]
        public int SupplierId { get; set; }

        [ForeignKey("SupplierId")]
        public virtual Supplier Supplier { get; set; }

        [Required(ErrorMessage = "المنتج مطلوب")]
        [Display(Name = "المنتج")]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        // مواصفات المنتج
        [Display(Name = "اتجاه الرول")]
        public int? RollDirectionId { get; set; }

        [ForeignKey("RollDirectionId")]
        public virtual RollDirection RollDirection { get; set; }

        [Required(ErrorMessage = "نوع الخام مطلوب")]
        [Display(Name = "نوع الخام")]
        public int RawMaterialId { get; set; }

        [ForeignKey("RawMaterialId")]
        public virtual RawMaterial RawMaterial { get; set; }

        [Required(ErrorMessage = "الطول مطلوب")]
        [Range(0.01, double.MaxValue, ErrorMessage = "الطول يجب أن يكون أكبر من صفر")]
        [Display(Name = "الطول (سم)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Length { get; set; }

        [Required(ErrorMessage = "العرض مطلوب")]
        [Range(0.01, double.MaxValue, ErrorMessage = "العرض يجب أن يكون أكبر من صفر")]
        [Display(Name = "العرض (سم)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Width { get; set; }

        [Required(ErrorMessage = "الكمية مطلوبة")]
        [Range(1, int.MaxValue, ErrorMessage = "الكمية يجب أن تكون على الأقل 1")]
        [Display(Name = "الكمية")]
        public int Quantity { get; set; }

        [StringLength(2000)]
        [Display(Name = "ملاحظات الطلب")]
        public string OrderNotes { get; set; }

        // ===== مرحلة المراجعة (Stage 2) =====

        [Display(Name = "الماكينة")]
        public int? MachineId { get; set; }

        [ForeignKey("MachineId")]
        public virtual Machine Machine { get; set; }

        [Display(Name = "القلب (Core)")]
        public int? CoreId { get; set; }

        [ForeignKey("CoreId")]
        public virtual Core Core { get; set; }

        [Display(Name = "السكينة")]
        public int? KnifeId { get; set; }

        [ForeignKey("KnifeId")]
        public virtual Knife Knife { get; set; }

        [Display(Name = "الكرتون")]
        public int? CartonId { get; set; }

        [ForeignKey("CartonId")]
        public virtual Carton Carton { get; set; }

        [Display(Name = "القالب")]
        public int? MoldId { get; set; }

        [ForeignKey("MoldId")]
        public virtual Mold Mold { get; set; }

        [Display(Name = "تاريخ المراجعة")]
        public DateTime? ReviewedDate { get; set; }

        [StringLength(450)]
        [Display(Name = "راجع بواسطة")]
        public string ReviewedBy { get; set; }

        [StringLength(2000)]
        [Display(Name = "ملاحظات المراجعة")]
        public string ReviewNotes { get; set; }

        // ===== مرحلة التصنيع (Stage 3) =====

        [Display(Name = "تاريخ بدء التصنيع")]
        public DateTime? ManufacturingStartDate { get; set; }

        [Display(Name = "تاريخ انتهاء التصنيع")]
        public DateTime? ManufacturingEndDate { get; set; }

        [StringLength(2000)]
        [Display(Name = "ملاحظات التصنيع")]
        public string ManufacturingNotes { get; set; } = string.Empty;

        // ===== مرحلة الطباعة (Stage 4) =====

        [Display(Name = "تاريخ بدء الطباعة")]
        public DateTime? PrintingStartDate { get; set; }

        [Display(Name = "تاريخ انتهاء الطباعة")]
        public DateTime? PrintingEndDate { get; set; }

        [StringLength(450)]
        [Display(Name = "طبع بواسطة")]
        public string PrintedBy { get; set; } = string.Empty;

        [StringLength(2000)]
        [Display(Name = "ملاحظات الطباعة")]
        public string PrintingNotes { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "الأولوية يجب أن تكون رقم موجب")]
        [Display(Name = "أولوية الطباعة")]
        public int Priority { get; set; } = 999;

        // ===== حالة ومرحلة الطلب =====

        [Required]
        [Display(Name = "حالة الطلب")]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [Required]
        [Display(Name = "مرحلة الطلب")]
        public OrderStage Stage { get; set; } = OrderStage.Order;

        // ===== Audit Fields =====

        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "آخر تعديل")]
        public DateTime? LastModified { get; set; }

        [Display(Name = "نشط")]
        public bool IsActive { get; set; } = true;

        [StringLength(450)]
        [Display(Name = "أنشئ بواسطة")]
        public string CreatedBy { get; set; } = string.Empty;

        [StringLength(450)]
        [Display(Name = "عدل بواسطة")]
        public string ModifiedBy { get; set; } = string.Empty;  

        // ===== Navigation Properties =====

        public virtual ICollection<OrderAttachment> Attachments { get; set; }
        public virtual ICollection<OrderManufacturingItem> ManufacturingItems { get; set; }
        public virtual ICollection<OrderTimeline> Timeline { get; set; }
    }

    // ===== Enums =====

    public enum OrderStatus
    {
        [Display(Name = "معلق")]
        Pending = 1,

        [Display(Name = "قيد المراجعة")]
        UnderReview = 2,

        [Display(Name = "قيد التصنيع")]
        InManufacturing = 3,

        [Display(Name = "قيد الطباعة")]
        InPrinting = 4,

        [Display(Name = "مكتمل")]
        Completed = 5,

        [Display(Name = "ملغي")]
        Cancelled = 6,

        [Display(Name = "معلق مؤقتاً")]
        OnHold = 7
    }

    public enum OrderStage
    {
        [Display(Name = "مرحلة الطلب")]
        Order = 1,

        [Display(Name = "مرحلة المراجعة")]
        Review = 2,

        [Display(Name = "مرحلة التصنيع")]
        Manufacturing = 3,

        [Display(Name = "مرحلة الطباعة")]
        Printing = 4,

        [Display(Name = "مكتمل")]
        Completed = 5
    }
}