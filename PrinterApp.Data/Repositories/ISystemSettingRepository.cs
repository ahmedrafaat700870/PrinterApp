using PrinterApp.Models.Entities;
namespace PrinterApp.Data.Repositories;

public interface ISystemSettingRepository : IRepository<SystemSetting>
{
    Task<SystemSetting> GetByKeyAsync(string key);
    Task<string> GetValueAsync(string key);
    Task SetValueAsync(string key, string value, string description = null);
    Task<Dictionary<string, string>> GetAllSettingsAsync();
}
