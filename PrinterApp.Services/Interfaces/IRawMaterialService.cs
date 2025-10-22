using PrinterApp.Models.ViewModels;

namespace PrinterApp.Services.Interfaces;

public interface IRawMaterialService
{
    Task<IEnumerable<RawMaterialViewModel>> GetAllRawMaterialsAsync();
    Task<IEnumerable<RawMaterialViewModel>> GetActiveRawMaterialsAsync();
    Task<IEnumerable<RawMaterialViewModel>> SearchRawMaterialsAsync(string searchTerm);
    Task<RawMaterialViewModel> GetRawMaterialByIdAsync(int id);
    Task<(bool Success, string[] Errors)> CreateRawMaterialAsync(RawMaterialViewModel model);
    Task<(bool Success, string[] Errors)> UpdateRawMaterialAsync(RawMaterialViewModel model);
    Task<(bool Success, string[] Errors)> DeleteRawMaterialAsync(int id);
    Task<(bool Success, string[] Errors)> ToggleRawMaterialStatusAsync(int id);
}