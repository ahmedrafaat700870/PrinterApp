using PrinterApp.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrinterApp.Services.Interfaces;

public interface ISettingsService
{
    Task<SettingsViewModel> GetSettingsAsync();
    Task<(bool Success, string[] Errors)> UpdateSettingsAsync(SettingsViewModel model);
    Task<string> GetSettingValueAsync(string key);
    Task SetSettingValueAsync(string key, string value);
}