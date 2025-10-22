using PrinterApp.Models.ViewModels;

namespace PrinterApp.Services.Interfaces;

public interface ISupplierService
{
    Task<IEnumerable<SupplierViewModel>> GetAllSuppliersAsync();
    Task<IEnumerable<SupplierViewModel>> GetActiveSuppliersAsync();
    Task<IEnumerable<SupplierViewModel>> SearchSuppliersAsync(string searchTerm);
    Task<SupplierViewModel> GetSupplierByIdAsync(int id);
    Task<SupplierViewModel> GetSupplierByCodeAsync(string code);
    Task<(bool Success, string[] Errors)> CreateSupplierAsync(SupplierViewModel model);
    Task<(bool Success, string[] Errors)> UpdateSupplierAsync(SupplierViewModel model);
    Task<(bool Success, string[] Errors)> DeleteSupplierAsync(int id);
    Task<(bool Success, string[] Errors)> ToggleSupplierStatusAsync(int id);
    Task<string> GetNextSupplierCodeAsync();
}