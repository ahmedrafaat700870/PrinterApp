using PrinterApp.Data.UnitOfWork;
using PrinterApp.Models.ViewModels;
using PrinterApp.Services.Interfaces;


namespace PrinterApp.Services.Implementations;

public class SettingsService : ISettingsService
{
    private readonly IUnitOfWork _unitOfWork;

    public SettingsService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<SettingsViewModel> GetSettingsAsync()
    {
        var settings = await _unitOfWork.SystemSettings.GetAllSettingsAsync();

        return new SettingsViewModel
        {
            ApplicationName = GetValue(settings, "ApplicationName", "Permission App"),
            DefaultLanguage = GetValue(settings, "DefaultLanguage", "en"),
            Theme = GetValue(settings, "Theme", "light"),
            ItemsPerPage = int.Parse(GetValue(settings, "ItemsPerPage", "10")),
            EnableEmailNotifications = bool.Parse(GetValue(settings, "EnableEmailNotifications", "true")),
            SessionTimeout = int.Parse(GetValue(settings, "SessionTimeout", "60")),
            DateFormat = GetValue(settings, "DateFormat", "dd/MM/yyyy"),
            TimeFormat = GetValue(settings, "TimeFormat", "HH:mm")
        };
    }

    public async Task<(bool Success, string[] Errors)> UpdateSettingsAsync(SettingsViewModel model)
    {
        try
        {
            await _unitOfWork.SystemSettings.SetValueAsync("ApplicationName", model.ApplicationName, "Application display name");
            await _unitOfWork.SystemSettings.SetValueAsync("DefaultLanguage", model.DefaultLanguage, "Default system language");
            await _unitOfWork.SystemSettings.SetValueAsync("Theme", model.Theme, "UI theme");
            await _unitOfWork.SystemSettings.SetValueAsync("ItemsPerPage", model.ItemsPerPage.ToString(), "Number of items per page");
            await _unitOfWork.SystemSettings.SetValueAsync("EnableEmailNotifications", model.EnableEmailNotifications.ToString(), "Enable email notifications");
            await _unitOfWork.SystemSettings.SetValueAsync("SessionTimeout", model.SessionTimeout.ToString(), "Session timeout in minutes");
            await _unitOfWork.SystemSettings.SetValueAsync("DateFormat", model.DateFormat, "Date display format");
            await _unitOfWork.SystemSettings.SetValueAsync("TimeFormat", model.TimeFormat, "Time display format");

            await _unitOfWork.CompleteAsync();
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, new[] { ex.Message });
        }
    }

    public async Task<string> GetSettingValueAsync(string key)
    {
        return await _unitOfWork.SystemSettings.GetValueAsync(key);
    }

    public async Task SetSettingValueAsync(string key, string value)
    {
        await _unitOfWork.SystemSettings.SetValueAsync(key, value);
        await _unitOfWork.CompleteAsync();
    }

    private string GetValue(Dictionary<string, string> settings, string key, string defaultValue)
    {
        return settings.ContainsKey(key) ? settings[key] : defaultValue;
    }
}