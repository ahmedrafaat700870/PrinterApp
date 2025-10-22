using PrinterApp.Models.ViewModels;

namespace PrinterApp.Services.Interfaces
{
    public interface IKnifeService
    {
        Task<IEnumerable<KnifeViewModel>> GetAllKnivesAsync();
        Task<IEnumerable<KnifeViewModel>> GetActiveKnivesAsync();
        Task<IEnumerable<KnifeViewModel>> SearchKnivesAsync(string searchTerm);
        Task<KnifeViewModel> GetKnifeByIdAsync(int id);
        Task<(bool Success, string[] Errors)> CreateKnifeAsync(KnifeViewModel model);
        Task<(bool Success, string[] Errors)> UpdateKnifeAsync(KnifeViewModel model);
        Task<(bool Success, string[] Errors)> DeleteKnifeAsync(int id);
        Task<(bool Success, string[] Errors)> ToggleKnifeStatusAsync(int id);
    }
}