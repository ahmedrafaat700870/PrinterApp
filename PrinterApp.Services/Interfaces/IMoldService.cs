using PrinterApp.Models.ViewModels;

namespace PrinterApp.Services.Interfaces
{
    public interface IMoldService
    {
        Task<IEnumerable<MoldViewModel>> GetAllMoldsAsync();
        Task<IEnumerable<MoldViewModel>> GetActiveMoldsAsync();
        Task<IEnumerable<MoldViewModel>> SearchMoldsAsync(string searchTerm);
        Task<MoldViewModel> GetMoldByIdAsync(int id);
        Task<MoldViewModel> GetMoldForCreateAsync();
        Task<MoldViewModel> GetMoldForEditAsync(int id);
        Task<(bool Success, string[] Errors)> CreateMoldAsync(MoldViewModel model);
        Task<(bool Success, string[] Errors)> UpdateMoldAsync(MoldViewModel model);
        Task<(bool Success, string[] Errors)> DeleteMoldAsync(int id);
        Task<(bool Success, string[] Errors)> ToggleMoldStatusAsync(int id);
        int CalculateTotalEyes(int width, int height);
    }
}