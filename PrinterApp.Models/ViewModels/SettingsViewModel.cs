using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrinterApp.Models.ViewModels;

public class SettingsViewModel
{
    [Display(Name = "Application Name")]
    [Required(ErrorMessage = "Application name is required")]
    public string ApplicationName { get; set; }

    [Display(Name = "Default Language")]
    public string DefaultLanguage { get; set; }

    [Display(Name = "Theme")]
    public string Theme { get; set; }

    [Display(Name = "Items Per Page")]
    [Range(5, 100, ErrorMessage = "Items per page must be between 5 and 100")]
    public int ItemsPerPage { get; set; }

    [Display(Name = "Enable Email Notifications")]
    public bool EnableEmailNotifications { get; set; }

    [Display(Name = "Session Timeout (minutes)")]
    [Range(5, 1440, ErrorMessage = "Session timeout must be between 5 and 1440 minutes")]
    public int SessionTimeout { get; set; }

    [Display(Name = "Date Format")]
    public string DateFormat { get; set; }

    [Display(Name = "Time Format")]
    public string TimeFormat { get; set; }
}