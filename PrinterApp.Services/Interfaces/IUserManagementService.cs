using PrinterApp.Models.ViewModels;
namespace PrinterApp.Services.Interfaces;

public interface IUserManagementService
{
    Task<List<UserRoleViewModel>> GetAllUsersAsync();
    Task<UserRoleViewModel> GetUserByIdAsync(string userId);
    Task<(bool Success, string[] Errors)> AssignRoleAsync(string userId, string roleName);
    Task<(bool Success, string[] Errors)> RemoveFromRoleAsync(string userId, string roleName);
    Task<List<string>> GetUserRolesAsync(string userId);
}
