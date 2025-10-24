using PrinterApp.Data.UnitOfWork;
using PrinterApp.Models.Entities;
using PrinterApp.Models.ViewModels;
using PrinterApp.Services.Interfaces;

namespace PrinterApp.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CustomerViewModel>> GetAllCustomersAsync()
        {
            var customers = await _unitOfWork.Customers.GetAllAsync();
            return customers.Select(MapToViewModel);
        }

        public async Task<IEnumerable<CustomerViewModel>> GetActiveCustomersAsync()
        {
            var customers = await _unitOfWork.Customers.GetActiveCustomersAsync();
            return customers.Select(MapToViewModel);
        }

        public async Task<CustomerViewModel> GetCustomerByIdAsync(int id)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            return customer != null ? MapToViewModel(customer) : null;
        }

        public async Task<CustomerViewModel> GetCustomerByCodeAsync(string customerCode)
        {
            var customer = await _unitOfWork.Customers.GetByCustomerCodeAsync(customerCode);
            return customer != null ? MapToViewModel(customer) : null;
        }

        public async Task<CustomerViewModel> GetCustomerWithOrdersAsync(int id)
        {
            var customer = await _unitOfWork.Customers.GetWithOrdersAsync(id);
            return customer != null ? MapToViewModel(customer) : null;
        }

        public async Task<(bool Success, string[] Errors)> CreateCustomerAsync(CustomerViewModel model, string userId)
        {
            var errors = new List<string>();

            // التحقق من وجود كود العميل
            if (await CustomerCodeExistsAsync(model.CustomerCode))
            {
                errors.Add("كود العميل موجود مسبقاً");
                return (false, errors.ToArray());
            }

            try
            {
                var customer = new Customer
                {
                    CustomerName = model.CustomerName,
                    CustomerCode = model.CustomerCode,
                    Phone = model.Phone,
                    Email = model.Email,
                    Address = model.Address,
                    City = model.City,
                    Governorate = model.Governorate,
                    Notes = model.Notes,
                    IsActive = true,
                    CreatedDate = DateTime.Now,
                    CreatedBy = userId
                };

                await _unitOfWork.Customers.AddAsync(customer);
                await _unitOfWork.CompleteAsync();

                return (true, Array.Empty<string>());
            }
            catch (Exception ex)
            {
                errors.Add($"حدث خطأ: {ex.Message}");
                return (false, errors.ToArray());
            }
        }

        public async Task<(bool Success, string[] Errors)> UpdateCustomerAsync(CustomerViewModel model, string userId)
        {
            var errors = new List<string>();

            var customer = await _unitOfWork.Customers.GetByIdAsync(model.Id);
            if (customer == null)
            {
                errors.Add("العميل غير موجود");
                return (false, errors.ToArray());
            }

            // التحقق من وجود كود العميل
            if (await CustomerCodeExistsAsync(model.CustomerCode, model.Id))
            {
                errors.Add("كود العميل موجود مسبقاً");
                return (false, errors.ToArray());
            }

            try
            {
                customer.CustomerName = model.CustomerName;
                customer.CustomerCode = model.CustomerCode;
                customer.Phone = model.Phone;
                customer.Email = model.Email;
                customer.Address = model.Address;
                customer.City = model.City;
                customer.Governorate = model.Governorate;
                customer.Notes = model.Notes;
                customer.LastModified = DateTime.Now;
                customer.ModifiedBy = userId;

                _unitOfWork.Customers.Update(customer);
                await _unitOfWork.CompleteAsync();

                return (true, Array.Empty<string>());
            }
            catch (Exception ex)
            {
                errors.Add($"حدث خطأ: {ex.Message}");
                return (false, errors.ToArray());
            }
        }

        public async Task<(bool Success, string[] Errors)> DeleteCustomerAsync(int id)
        {
            var errors = new List<string>();

            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer == null)
            {
                errors.Add("العميل غير موجود");
                return (false, errors.ToArray());
            }

            try
            {
                _unitOfWork.Customers.Delete(customer);
                await _unitOfWork.CompleteAsync();

                return (true, Array.Empty<string>());
            }
            catch (Exception ex)
            {
                errors.Add($"حدث خطأ: {ex.Message}");
                return (false, errors.ToArray());
            }
        }

        public async Task<(bool Success, string[] Errors)> ToggleCustomerStatusAsync(int id)
        {
            var errors = new List<string>();

            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer == null)
            {
                errors.Add("العميل غير موجود");
                return (false, errors.ToArray());
            }

            try
            {
                customer.IsActive = !customer.IsActive;
                customer.LastModified = DateTime.Now;

                _unitOfWork.Customers.Update(customer);
                await _unitOfWork.CompleteAsync();

                return (true, Array.Empty<string>());
            }
            catch (Exception ex)
            {
                errors.Add($"حدث خطأ: {ex.Message}");
                return (false, errors.ToArray());
            }
        }

        public async Task<IEnumerable<CustomerViewModel>> SearchCustomersAsync(string searchTerm)
        {
            var customers = await _unitOfWork.Customers.SearchAsync(searchTerm);
            return customers.Select(MapToViewModel);
        }

        public async Task<bool> CustomerCodeExistsAsync(string customerCode, int? excludeId = null)
        {
            return await _unitOfWork.Customers.CustomerCodeExistsAsync(customerCode, excludeId);
        }

        public async Task<int> GetTotalCustomersCountAsync()
        {
            return await _unitOfWork.Customers.GetTotalCustomersCountAsync();
        }

        public async Task<int> GetActiveCustomersCountAsync()
        {
            return await _unitOfWork.Customers.GetActiveCustomersCountAsync();
        }

        // Helper Method
        private CustomerViewModel MapToViewModel(Customer customer)
        {
            return new CustomerViewModel
            {
                Id = customer.Id,
                CustomerName = customer.CustomerName,
                CustomerCode = customer.CustomerCode,
                Phone = customer.Phone,
                Email = customer.Email,
                Address = customer.Address,
                City = customer.City,
                Governorate = customer.Governorate,
                Notes = customer.Notes,
                IsActive = customer.IsActive,
                CreatedDate = customer.CreatedDate,
                LastModified = customer.LastModified,
                OrdersCount = customer.Orders?.Count ?? 0
            };
        }
    }
}