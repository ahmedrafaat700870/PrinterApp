using PrinterApp.Models.ViewModels;

namespace PrinterApp.Services.Interfaces
{
    public interface ICartonService
    {
        Task<IEnumerable<CartonViewModel>> GetAllCartonsAsync();
        Task<IEnumerable<CartonViewModel>> GetActiveCartonsAsync();
        Task<IEnumerable<CartonViewModel>> SearchCartonsAsync(string searchTerm);
        Task<CartonViewModel> GetCartonByIdAsync(int id);
        Task<(bool Success, string[] Errors)> CreateCartonAsync(CartonViewModel model);
        Task<(bool Success, string[] Errors)> UpdateCartonAsync(CartonViewModel model);
        Task<(bool Success, string[] Errors)> DeleteCartonAsync(int id);
        Task<(bool Success, string[] Errors)> ToggleCartonStatusAsync(int id);
    }
}