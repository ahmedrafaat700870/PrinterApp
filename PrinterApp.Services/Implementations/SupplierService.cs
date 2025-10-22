using PrinterApp.Data.UnitOfWork;
using PrinterApp.Models.Entities;
using PrinterApp.Models.ViewModels;
using PrinterApp.Services.Interfaces;

namespace PrinterApp.Services.Implementations;

public class SupplierService : ISupplierService
{
    private readonly IUnitOfWork _unitOfWork;

    public SupplierService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<SupplierViewModel>> GetAllSuppliersAsync()
    {
        var suppliers = await _unitOfWork.Suppliers.GetAllAsync();
        return suppliers.Select(MapToViewModel).OrderBy(s => s.SupplierCode);
    }

    public async Task<IEnumerable<SupplierViewModel>> GetActiveSuppliersAsync()
    {
        var suppliers = await _unitOfWork.Suppliers.GetActiveSuppliersAsync();
        return suppliers.Select(MapToViewModel);
    }

    public async Task<IEnumerable<SupplierViewModel>> SearchSuppliersAsync(string searchTerm)
    {
        var suppliers = await _unitOfWork.Suppliers.GetAllAsync();

        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return suppliers.Select(MapToViewModel).OrderBy(s => s.SupplierCode);
        }

        searchTerm = searchTerm.ToLower().Trim();

        var filteredSuppliers = suppliers.Where(s =>
            s.SupplierCode.Contains(searchTerm) ||
            s.SupplierName.ToLower().Contains(searchTerm) ||
            (!string.IsNullOrEmpty(s.PhoneNumber) && s.PhoneNumber.Contains(searchTerm)) ||
            (!string.IsNullOrEmpty(s.CardNumber) && s.CardNumber.ToLower().Contains(searchTerm)) ||
            (!string.IsNullOrEmpty(s.CommercialRegister) && s.CommercialRegister.ToLower().Contains(searchTerm)) ||
            (!string.IsNullOrEmpty(s.Email) && s.Email.ToLower().Contains(searchTerm)) ||
            (!string.IsNullOrEmpty(s.City) && s.City.ToLower().Contains(searchTerm))
        );

        return filteredSuppliers.Select(MapToViewModel).OrderBy(s => s.SupplierCode);
    }

    public async Task<SupplierViewModel> GetSupplierByIdAsync(int id)
    {
        var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);
        return supplier != null ? MapToViewModel(supplier) : null;
    }

    public async Task<SupplierViewModel> GetSupplierByCodeAsync(string code)
    {
        var supplier = await _unitOfWork.Suppliers.GetByCodeAsync(code);
        return supplier != null ? MapToViewModel(supplier) : null;
    }

    public async Task<(bool Success, string[] Errors)> CreateSupplierAsync(SupplierViewModel model)
    {
        try
        {
            // Check if supplier name already exists
            if (await _unitOfWork.Suppliers.SupplierNameExistsAsync(model.SupplierName))
            {
                return (false, new[] { "A supplier with this name already exists" });
            }

            // Get next supplier code
            var nextCode = await _unitOfWork.Suppliers.GetNextSupplierCodeAsync();

            var supplier = new Supplier
            {
                SupplierCode = nextCode,
                SupplierName = model.SupplierName,
                CardNumber = model.CardNumber,
                CommercialRegister = model.CommercialRegister,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                Address = model.Address,
                City = model.City,
                Country = model.Country,
                Notes = model.Notes,
                CreatedDate = DateTime.Now,
                IsActive = true
            };

            await _unitOfWork.Suppliers.AddAsync(supplier);
            await _unitOfWork.CompleteAsync();

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, new[] { $"Error creating supplier: {ex.Message}" });
        }
    }

    public async Task<(bool Success, string[] Errors)> UpdateSupplierAsync(SupplierViewModel model)
    {
        try
        {
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(model.Id);
            if (supplier == null)
            {
                return (false, new[] { "Supplier not found" });
            }

            // Check if new name conflicts with existing supplier
            if (await _unitOfWork.Suppliers.SupplierNameExistsAsync(model.SupplierName, model.Id))
            {
                return (false, new[] { "A supplier with this name already exists" });
            }

            supplier.SupplierName = model.SupplierName;
            supplier.CardNumber = model.CardNumber;
            supplier.CommercialRegister = model.CommercialRegister;
            supplier.PhoneNumber = model.PhoneNumber;
            supplier.Email = model.Email;
            supplier.Address = model.Address;
            supplier.City = model.City;
            supplier.Country = model.Country;
            supplier.Notes = model.Notes;
            supplier.LastModified = DateTime.Now;
            supplier.IsActive = model.IsActive;

            _unitOfWork.Suppliers.Update(supplier);
            await _unitOfWork.CompleteAsync();

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, new[] { $"Error updating supplier: {ex.Message}" });
        }
    }

    public async Task<(bool Success, string[] Errors)> DeleteSupplierAsync(int id)
    {
        try
        {
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);
            if (supplier == null)
            {
                return (false, new[] { "Supplier not found" });
            }

            _unitOfWork.Suppliers.Delete(supplier);
            await _unitOfWork.CompleteAsync();

            // Reassign supplier codes after deletion
            await _unitOfWork.Suppliers.ReassignSupplierCodesAsync();

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, new[] { $"Error deleting supplier: {ex.Message}" });
        }
    }

    public async Task<(bool Success, string[] Errors)> ToggleSupplierStatusAsync(int id)
    {
        try
        {
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);
            if (supplier == null)
            {
                return (false, new[] { "Supplier not found" });
            }

            supplier.IsActive = !supplier.IsActive;
            supplier.LastModified = DateTime.Now;

            _unitOfWork.Suppliers.Update(supplier);
            await _unitOfWork.CompleteAsync();

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, new[] { $"Error toggling supplier status: {ex.Message}" });
        }
    }

    public async Task<string> GetNextSupplierCodeAsync()
    {
        return await _unitOfWork.Suppliers.GetNextSupplierCodeAsync();
    }

    private SupplierViewModel MapToViewModel(Supplier supplier)
    {
        return new SupplierViewModel
        {
            Id = supplier.Id,
            SupplierCode = supplier.SupplierCode,
            SupplierName = supplier.SupplierName,
            CardNumber = supplier.CardNumber,
            CommercialRegister = supplier.CommercialRegister,
            PhoneNumber = supplier.PhoneNumber,
            Email = supplier.Email,
            Address = supplier.Address,
            City = supplier.City,
            Country = supplier.Country,
            Notes = supplier.Notes,
            CreatedDate = supplier.CreatedDate,
            LastModified = supplier.LastModified,
            IsActive = supplier.IsActive
        };
    }
}