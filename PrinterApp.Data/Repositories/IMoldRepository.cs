using PrinterApp.Models.Entities;

namespace PrinterApp.Data.Repositories
{
    public interface IMoldRepository : IRepository<Mold>
    {
        Task<List<Mold>> GetActiveMoldsAsync();
        Task<List<Mold>> GetMoldsWithDetailsAsync();
        Task<Mold> GetMoldWithDetailsAsync(int id);
        Task<Mold> GetByNumberAsync(string moldNumber);
        Task<bool> MoldNumberExistsAsync(string moldNumber, int? excludeId = null);
        Task<List<Mold>> GetMoldsByMachineAsync(int machineId);
        Task<List<Mold>> GetMoldsByShapeAsync(int shapeId);
    }
}