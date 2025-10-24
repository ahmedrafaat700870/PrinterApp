using Microsoft.EntityFrameworkCore;
using PrinterApp.Data;
using PrinterApp.Models.Entities;

namespace PrinterApp.Data.Repositories
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Customer> GetByCustomerCodeAsync(string customerCode)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerCode == customerCode);
        }

        public async Task<bool> CustomerCodeExistsAsync(string customerCode, int? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return await _context.Customers
                    .AnyAsync(c => c.CustomerCode == customerCode && c.Id != excludeId.Value);
            }
            return await _context.Customers.AnyAsync(c => c.CustomerCode == customerCode);
        }

        public async Task<IEnumerable<Customer>> GetByPhoneAsync(string phone)
        {
            return await _context.Customers
                .Where(c => c.Phone.Contains(phone) && c.IsActive)
                .OrderBy(c => c.CustomerName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Customer>> SearchAsync(string searchTerm)
        {
            return await _context.Customers
                .Where(c => c.IsActive &&
                    (c.CustomerName.Contains(searchTerm) ||
                     c.CustomerCode.Contains(searchTerm) ||
                     c.Phone.Contains(searchTerm) ||
                     c.Email.Contains(searchTerm)))
                .OrderBy(c => c.CustomerName)
                .ToListAsync();
        }

        public async Task<Customer> GetWithOrdersAsync(int id)
        {
            return await _context.Customers
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Customer>> GetActiveCustomersAsync()
        {
            return await _context.Customers
                .Where(c => c.IsActive)
                .OrderBy(c => c.CustomerName)
                .ToListAsync();
        }

        public async Task<int> GetTotalCustomersCountAsync()
        {
            return await _context.Customers.CountAsync();
        }

        public async Task<int> GetActiveCustomersCountAsync()
        {
            return await _context.Customers.CountAsync(c => c.IsActive);
        }

        public  async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _context.Customers
                .OrderBy(c => c.CustomerName)
                .ToListAsync();
        }
    }
}