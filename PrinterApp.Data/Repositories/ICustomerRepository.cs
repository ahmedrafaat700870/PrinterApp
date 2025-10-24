using PrinterApp.Models.Entities;

namespace PrinterApp.Data.Repositories
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer> GetByCustomerCodeAsync(string customerCode);
        Task<bool> CustomerCodeExistsAsync(string customerCode, int? excludeId = null);
        Task<IEnumerable<Customer>> GetByPhoneAsync(string phone);
        Task<IEnumerable<Customer>> SearchAsync(string searchTerm);
        Task<Customer> GetWithOrdersAsync(int id);
        Task<IEnumerable<Customer>> GetActiveCustomersAsync();
        Task<int> GetTotalCustomersCountAsync();
        Task<int> GetActiveCustomersCountAsync();
    }
}