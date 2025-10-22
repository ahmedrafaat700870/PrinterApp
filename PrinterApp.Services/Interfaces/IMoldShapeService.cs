using PrinterApp.Models.ViewModels;

namespace PrinterApp.Services.Interfaces;

public interface IMoldShapeService
{
    Task<IEnumerable<MoldShapeViewModel>> GetAllShapesAsync();
    Task<IEnumerable<MoldShapeViewModel>> GetActiveShapesAsync();
    Task<IEnumerable<MoldShapeViewModel>> SearchShapesAsync(string searchTerm);
    Task<MoldShapeViewModel> GetShapeByIdAsync(int id);
    Task<(bool Success, string[] Errors)> CreateShapeAsync(MoldShapeViewModel model, string imagePath);
    Task<(bool Success, string[] Errors)> UpdateShapeAsync(MoldShapeViewModel model, string imagePath);
    Task<(bool Success, string[] Errors)> DeleteShapeAsync(int id);
    Task<(bool Success, string[] Errors)> ToggleShapeStatusAsync(int id);
}