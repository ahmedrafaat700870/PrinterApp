using PrinterApp.Models.Entities;

namespace PrinterApp.Data.Repositories
{
    public interface IMachineRepository : IRepository<Machine>
    {
        Task<List<Machine>> GetActiveMachinesAsync();
        Task<Machine> GetByNameAsync(string machineName);
        Task<bool> MachineNameExistsAsync(string machineName, int? excludeId = null);
        Task<List<Machine>> GetByManufacturerAsync(string manufacturer);
    }
}