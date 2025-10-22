using PrinterApp.Models.Entities;

namespace PrinterApp.Data.Repositories
{
    public interface IManufacturingAdditionRepository : IRepository<ManufacturingAddition>
    {
        Task<List<ManufacturingAddition>> GetActiveAdditionsAsync();
        Task<ManufacturingAddition> GetByNameAsync(string additionName);
        Task<bool> AdditionNameExistsAsync(string additionName, int? excludeId = null);
    }
}