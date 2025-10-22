using PrinterApp.Models.ViewModels;

namespace PrinterApp.Services.Interfaces
{
    public interface IMachineService
    {
        Task<IEnumerable<MachineViewModel>> GetAllMachinesAsync();
        Task<IEnumerable<MachineViewModel>> GetActiveMachinesAsync();
        Task<IEnumerable<MachineViewModel>> SearchMachinesAsync(string searchTerm);
        Task<MachineViewModel> GetMachineByIdAsync(int id);
        Task<(bool Success, string[] Errors)> CreateMachineAsync(MachineViewModel model);
        Task<(bool Success, string[] Errors)> UpdateMachineAsync(MachineViewModel model);
        Task<(bool Success, string[] Errors)> DeleteMachineAsync(int id);
        Task<(bool Success, string[] Errors)> ToggleMachineStatusAsync(int id);
    }
}