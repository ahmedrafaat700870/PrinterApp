using System.ComponentModel.DataAnnotations;

namespace PrinterApp.Models.Entities
{
    public class Customer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم العميل مطلوب")]
        [StringLength(100, ErrorMessage = "اسم العميل لا يمكن أن يتجاوز 100 حرف")]
        [Display(Name = "اسم العميل")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "كود العميل مطلوب")]
        [StringLength(50, ErrorMessage = "كود العميل لا يمكن أن يتجاوز 50 حرف")]
        [Display(Name = "كود العميل")]
        public string CustomerCode { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [StringLength(20, ErrorMessage = "رقم الهاتف لا يمكن أن يتجاوز 20 حرف")]
        [Display(Name = "رقم الهاتف")]
        [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
        public string Phone { get; set; }

        [StringLength(100)]
        [Display(Name = "البريد الإلكتروني")]
        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صحيح")]
        public string Email { get; set; }

        [StringLength(500)]
        [Display(Name = "العنوان")]
        public string Address { get; set; }

        [StringLength(200)]
        [Display(Name = "المدينة")]
        public string City { get; set; }

        [StringLength(200)]
        [Display(Name = "المحافظة")]
        public string Governorate { get; set; }

        [StringLength(1000)]
        [Display(Name = "ملاحظات")]
        public string Notes { get; set; }

        [Display(Name = "نشط")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "آخر تعديل")]
        public DateTime? LastModified { get; set; }

        [StringLength(450)]
        [Display(Name = "أنشئ بواسطة")]
        public string CreatedBy { get; set; }

        [StringLength(450)]
        [Display(Name = "عدل بواسطة")]
        public string ModifiedBy { get; set; }

        // Navigation Properties
        public virtual ICollection<Order> Orders { get; set; }
    }
}