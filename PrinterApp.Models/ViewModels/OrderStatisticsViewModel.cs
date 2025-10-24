using System.ComponentModel.DataAnnotations;

namespace PrinterApp.Models.ViewModels
{
    /// <summary>
    /// ViewModel لإحصائيات الطلبات
    /// </summary>
    public class OrderStatisticsViewModel
    {
        // =====================================================
        // إحصائيات عامة
        // =====================================================

        [Display(Name = "إجمالي الطلبات")]
        public int TotalOrders { get; set; }

        [Display(Name = "الطلبات النشطة")]
        public int ActiveOrders { get; set; }

        [Display(Name = "الطلبات غير النشطة")]
        public int InactiveOrders { get; set; }

        // =====================================================
        // إحصائيات حسب الحالة (Status)
        // =====================================================

        [Display(Name = "طلبات معلقة")]
        public int PendingOrders { get; set; }

        [Display(Name = "تحت المراجعة")]
        public int UnderReviewOrders { get; set; }

        [Display(Name = "قيد التنفيذ")]
        public int InProgressOrders { get; set; }

        [Display(Name = "قيد التصنيع")]
        public int InManufacturingOrders { get; set; }

        [Display(Name = "قيد الطباعة")]
        public int InPrintingOrders { get; set; }

        [Display(Name = "طلبات مكتملة")]
        public int CompletedOrders { get; set; }

        [Display(Name = "طلبات ملغية")]
        public int CancelledOrders { get; set; }

        [Display(Name = "طلبات معلقة مؤقتاً")]
        public int OnHoldOrders { get; set; }

        // =====================================================
        // إحصائيات حسب المرحلة (Stage)
        // =====================================================

        [Display(Name = "مرحلة الطلب")]
        public int OrderStageCount { get; set; }

        [Display(Name = "مرحلة المراجعة")]
        public int ReviewStageCount { get; set; }

        [Display(Name = "مرحلة التصنيع")]
        public int ManufacturingStageCount { get; set; }

        [Display(Name = "مرحلة الطباعة")]
        public int PrintingStageCount { get; set; }

        [Display(Name = "مرحلة مكتملة")]
        public int CompletedStageCount { get; set; }

        // =====================================================
        // إحصائيات التأخير
        // =====================================================

        [Display(Name = "طلبات متأخرة")]
        public int LateOrders { get; set; }

        [Display(Name = "نسبة الطلبات المتأخرة")]
        public decimal LateOrdersPercentage { get; set; }

        // =====================================================
        // إحصائيات اليوم
        // =====================================================

        [Display(Name = "طلبات اليوم")]
        public int TodayOrders { get; set; }

        [Display(Name = "مكتمل اليوم")]
        public int TodayCompletedOrders { get; set; }

        [Display(Name = "ملغي اليوم")]
        public int TodayCancelledOrders { get; set; }

        // =====================================================
        // إحصائيات الأسبوع
        // =====================================================

        [Display(Name = "طلبات الأسبوع")]
        public int WeekOrders { get; set; }

        [Display(Name = "مكتمل الأسبوع")]
        public int WeekCompletedOrders { get; set; }

        // =====================================================
        // إحصائيات الشهر
        // =====================================================

        [Display(Name = "طلبات الشهر")]
        public int MonthOrders { get; set; }

        [Display(Name = "مكتمل الشهر")]
        public int MonthCompletedOrders { get; set; }

        // =====================================================
        // إحصائيات السنة
        // =====================================================

        [Display(Name = "طلبات السنة")]
        public int YearOrders { get; set; }

        [Display(Name = "مكتمل السنة")]
        public int YearCompletedOrders { get; set; }

        // =====================================================
        // متوسطات
        // =====================================================

        [Display(Name = "متوسط الطلبات اليومية")]
        public decimal AverageDailyOrders { get; set; }

        [Display(Name = "متوسط وقت الإنجاز (بالأيام)")]
        public decimal AverageCompletionTime { get; set; }

        [Display(Name = "نسبة الإنجاز")]
        public decimal CompletionRate { get; set; }

        // =====================================================
        // إحصائيات إضافية
        // =====================================================

        [Display(Name = "إجمالي الكمية")]
        public int TotalQuantity { get; set; }

        [Display(Name = "إجمالي الكمية المكتملة")]
        public int CompletedQuantity { get; set; }

        [Display(Name = "إجمالي الكمية المعلقة")]
        public int PendingQuantity { get; set; }

        // =====================================================
        // Computed Properties
        // =====================================================

        /// <summary>
        /// نسبة الطلبات المكتملة
        /// </summary>
        public decimal CompletedPercentage => TotalOrders > 0
            ? Math.Round((decimal)CompletedOrders / TotalOrders * 100, 2)
            : 0;

        /// <summary>
        /// نسبة الطلبات الملغية
        /// </summary>
        public decimal CancelledPercentage => TotalOrders > 0
            ? Math.Round((decimal)CancelledOrders / TotalOrders * 100, 2)
            : 0;

        /// <summary>
        /// نسبة الطلبات قيد التنفيذ
        /// </summary>
        public decimal InProgressPercentage => TotalOrders > 0
            ? Math.Round((decimal)InProgressOrders / TotalOrders * 100, 2)
            : 0;

        /// <summary>
        /// نسبة إنجاز الكمية
        /// </summary>
        public decimal QuantityCompletionPercentage => TotalQuantity > 0
            ? Math.Round((decimal)CompletedQuantity / TotalQuantity * 100, 2)
            : 0;
    }
}