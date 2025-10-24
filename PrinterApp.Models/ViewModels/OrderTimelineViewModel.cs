using PrinterApp.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace PrinterApp.Models.ViewModels
{
    public class OrderTimelineViewModel
    {
        public int Id { get; set; }

        [Display(Name = "المرحلة")]
        public OrderStage Stage { get; set; }

        [Display(Name = "الحالة")]
        public OrderStatus Status { get; set; }

        [Display(Name = "الإجراء")]
        public string Action { get; set; }

        [Display(Name = "ملاحظات")]
        public string Notes { get; set; }

        [Display(Name = "التاريخ")]
        public DateTime ActionDate { get; set; }

        [Display(Name = "المستخدم")]
        public string ActionByName { get; set; }

        // للعرض
        public string StageText => Stage.GetDisplayName();
        public string StatusText => Status.GetDisplayName();
        public string TimeAgo => GetTimeAgo();
        public string StageIcon => GetStageIcon();
        public string StageBadgeClass => GetStageBadgeClass();

        private string GetTimeAgo()
        {
            var timeSpan = DateTime.Now - ActionDate;

            if (timeSpan.TotalMinutes < 1)
                return "الآن";
            if (timeSpan.TotalMinutes < 60)
                return $"منذ {(int)timeSpan.TotalMinutes} دقيقة";
            if (timeSpan.TotalHours < 24)
                return $"منذ {(int)timeSpan.TotalHours} ساعة";
            if (timeSpan.TotalDays < 30)
                return $"منذ {(int)timeSpan.TotalDays} يوم";

            return ActionDate.ToString("dd/MM/yyyy");
        }

        private string GetStageIcon()
        {
            return Stage switch
            {
                OrderStage.Order => "fa-clipboard-list",
                OrderStage.Review => "fa-search",
                OrderStage.Manufacturing => "fa-industry",
                OrderStage.Printing => "fa-print",
                OrderStage.Completed => "fa-check-circle",
                _ => "fa-circle"
            };
        }

        private string GetStageBadgeClass()
        {
            return Stage switch
            {
                OrderStage.Order => "badge bg-warning",
                OrderStage.Review => "badge bg-info",
                OrderStage.Manufacturing => "badge bg-primary",
                OrderStage.Printing => "badge bg-secondary",
                OrderStage.Completed => "badge bg-success",
                _ => "badge bg-secondary"
            };
        }
    }
}