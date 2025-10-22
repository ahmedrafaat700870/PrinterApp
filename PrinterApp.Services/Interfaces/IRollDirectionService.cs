using PrinterApp.Models.ViewModels;

namespace PrinterApp.Services.Interfaces
{
    public interface IRollDirectionService
    {
        Task<IEnumerable<RollDirectionViewModel>> GetAllDirectionsAsync();
        Task<IEnumerable<RollDirectionViewModel>> GetActiveDirectionsAsync();
        Task<IEnumerable<RollDirectionViewModel>> SearchDirectionsAsync(string searchTerm);
        Task<RollDirectionViewModel> GetDirectionByIdAsync(int id);
        Task<(bool Success, string[] Errors)> CreateDirectionAsync(RollDirectionViewModel model, string imagePath);
        Task<(bool Success, string[] Errors)> UpdateDirectionAsync(RollDirectionViewModel model, string imagePath);
        Task<(bool Success, string[] Errors)> DeleteDirectionAsync(int id, string deleteImagePath);
        Task<(bool Success, string[] Errors)> ToggleDirectionStatusAsync(int id);
    }
}