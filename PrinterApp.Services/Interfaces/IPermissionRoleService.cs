using PrinterApp.Models.ViewModels;
namespace PrinterApp.Services.Interfaces;

public interface IPermissionRoleService
{
    Task<List<PermissionRoleViewModel>> GetRolesByPermissionIdAsync(int permissionId);
    Task<(bool Success, string[] Errors)> CreateRoleAsync(int permissionId, PermissionRoleViewModel model);
    Task<(bool Success, string[] Errors)> DeleteRoleAsync(int roleId);
}