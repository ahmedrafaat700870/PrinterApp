using PrinterApp.Models.Entities;

namespace PrinterApp.Data.Repositories
{
    public interface IKnifeRepository : IRepository<Knife>
    {
        Task<List<Knife>> GetActiveKnivesAsync();
        Task<Knife> GetByNameAsync(string knifeName);
        Task<bool> KnifeNameExistsAsync(string knifeName, int? excludeId = null);
        Task<List<Knife>> GetByFactorRangeAsync(decimal minFactor, decimal maxFactor);
    }
}