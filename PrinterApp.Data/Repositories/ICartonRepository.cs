using PrinterApp.Models.Entities;

namespace PrinterApp.Data.Repositories
{
    public interface ICartonRepository : IRepository<Carton>
    {
        Task<List<Carton>> GetActiveCartonsAsync();
        Task<Carton> GetByNameAsync(string cartonName);
        Task<bool> CartonNameExistsAsync(string cartonName, int? excludeId = null);
        Task<List<Carton>> GetByFactorRangeAsync(decimal minFactor, decimal maxFactor);
    }
}