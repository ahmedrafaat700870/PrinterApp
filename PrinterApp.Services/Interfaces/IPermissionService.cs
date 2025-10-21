using PrinterApp.Models.ViewModels;
namespace PrinterApp.Services.Interfaces;

public interface IPermissionService
{
    Task<IEnumerable<PermissionViewModel>> GetAllPermissionsAsync();
    Task<PermissionViewModel> GetPermissionByIdAsync(int id);
    Task<PermissionViewModel> GetPermissionWithRolesAsync(int id);
    Task<(bool Success, string[] Errors)> CreatePermissionAsync(PermissionViewModel model);
    Task<(bool Success, string[] Errors)> UpdatePermissionAsync(PermissionViewModel model);
    Task<bool> DeletePermissionAsync(int id);
}