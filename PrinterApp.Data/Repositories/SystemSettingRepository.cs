using Microsoft.EntityFrameworkCore;
using PrinterApp.Models.Entities;


namespace PrinterApp.Data.Repositories;

public class SystemSettingRepository : Repository<SystemSetting>, ISystemSettingRepository
{
    public SystemSettingRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<SystemSetting> GetByKeyAsync(string key)
    {
        return await _dbSet.FirstOrDefaultAsync(s => s.Key == key);
    }

    public async Task<string> GetValueAsync(string key)
    {
        var setting = await GetByKeyAsync(key);
        return setting?.Value;
    }

    public async Task SetValueAsync(string key, string value, string description = null)
    {
        var setting = await GetByKeyAsync(key);

        if (setting == null)
        {
            setting = new SystemSetting
            {
                Key = key,
                Value = value,
                Description = description,
                LastModified = DateTime.Now
            };
            await _dbSet.AddAsync(setting);
        }
        else
        {
            setting.Value = value;
            if (!string.IsNullOrEmpty(description))
            {
                setting.Description = description;
            }
            setting.LastModified = DateTime.Now;
            _dbSet.Update(setting);
        }
    }

    public async Task<Dictionary<string, string>> GetAllSettingsAsync()
    {
        var settings = await _dbSet.ToListAsync();
        return settings.ToDictionary(s => s.Key, s => s.Value);
    }
}
