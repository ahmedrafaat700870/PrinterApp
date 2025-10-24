using PrinterApp.Models.ViewModels;

namespace PrinterApp.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerViewModel>> GetAllCustomersAsync();
        Task<IEnumerable<CustomerViewModel>> GetActiveCustomersAsync();
        Task<CustomerViewModel> GetCustomerByIdAsync(int id);
        Task<CustomerViewModel> GetCustomerByCodeAsync(string customerCode);
        Task<CustomerViewModel> GetCustomerWithOrdersAsync(int id);
        Task<(bool Success, string[] Errors)> CreateCustomerAsync(CustomerViewModel model, string userId);
        Task<(bool Success, string[] Errors)> UpdateCustomerAsync(CustomerViewModel model, string userId);
        Task<(bool Success, string[] Errors)> DeleteCustomerAsync(int id);
        Task<(bool Success, string[] Errors)> ToggleCustomerStatusAsync(int id);
        Task<IEnumerable<CustomerViewModel>> SearchCustomersAsync(string searchTerm);
        Task<bool> CustomerCodeExistsAsync(string customerCode, int? excludeId = null);
        Task<int> GetTotalCustomersCountAsync();
        Task<int> GetActiveCustomersCountAsync();
    }
}