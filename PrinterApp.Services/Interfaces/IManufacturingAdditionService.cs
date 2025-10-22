using PrinterApp.Models.ViewModels;

namespace PrinterApp.Services.Interfaces
{
    public interface IManufacturingAdditionService
    {
        Task<IEnumerable<ManufacturingAdditionViewModel>> GetAllAdditionsAsync();
        Task<IEnumerable<ManufacturingAdditionViewModel>> GetActiveAdditionsAsync();
        Task<IEnumerable<ManufacturingAdditionViewModel>> SearchAdditionsAsync(string searchTerm);
        Task<ManufacturingAdditionViewModel> GetAdditionByIdAsync(int id);
        Task<(bool Success, string[] Errors)> CreateAdditionAsync(ManufacturingAdditionViewModel model);
        Task<(bool Success, string[] Errors)> UpdateAdditionAsync(ManufacturingAdditionViewModel model);
        Task<(bool Success, string[] Errors)> DeleteAdditionAsync(int id);
        Task<(bool Success, string[] Errors)> ToggleAdditionStatusAsync(int id);
    }
}