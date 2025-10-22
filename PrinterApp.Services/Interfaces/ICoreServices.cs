

using PrinterApp.Models.ViewModels;

namespace PrinterApp.Services.Interfaces;

public interface ICoreService
{
    Task<IEnumerable<CoreViewModel>> GetAllCoresAsync();
    Task<IEnumerable<CoreViewModel>> GetActiveCoresAsync();
    Task<CoreViewModel> GetCoreByIdAsync(int id);
    Task<(bool Success, string[] Errors)> CreateCoreAsync(CoreViewModel model);
    Task<(bool Success, string[] Errors)> UpdateCoreAsync(CoreViewModel model);
    Task<(bool Success, string[] Errors)> DeleteCoreAsync(int id);
    Task<(bool Success, string[] Errors)> ToggleCoreStatusAsync(int id);
    Task<IEnumerable<CoreViewModel>> SearchCoresAsync(string searchTerm); 
}