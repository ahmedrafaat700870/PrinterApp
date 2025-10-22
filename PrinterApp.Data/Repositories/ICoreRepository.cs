using PrinterApp.Models.Entities;
namespace PrinterApp.Data.Repositories;

public interface ICoreRepository : IRepository<Core>
{
    Task<List<Core>> GetActiveCoresAsync();
    Task<Core> GetCoreByName(string coreName);
    Task<bool> CoreNameExistsAsync(string coreName , int? excludeId = null);
}
