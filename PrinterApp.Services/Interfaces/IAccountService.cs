using PrinterApp.Models.ViewModels;
namespace PrinterApp.Services.Interfaces;

public interface IAccountService
{
    Task<(bool Success, string[] Errors)> RegisterAsync(RegisterViewModel model);
    Task<bool> LoginAsync(LoginViewModel model);
    Task LogoutAsync();
}
