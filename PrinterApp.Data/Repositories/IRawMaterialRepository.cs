using PrinterApp.Models.Entities;

namespace PrinterApp.Data.Repositories;

public interface IRawMaterialRepository : IRepository<RawMaterial>
{
    Task<List<RawMaterial>> GetActiveRawMaterialsAsync();
    Task<RawMaterial> GetByNameAsync(string rawMaterialName);
    Task<bool> RawMaterialNameExistsAsync(string rawMaterialName, int? excludeId = null);
}