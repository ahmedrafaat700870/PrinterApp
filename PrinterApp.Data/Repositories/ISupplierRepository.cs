using PrinterApp.Models.Entities;


namespace PrinterApp.Data.Repositories;

public interface ISupplierRepository : IRepository<Supplier>
{
    Task<List<Supplier>> GetActiveSuppliersAsync();
    Task<Supplier> GetByCodeAsync(string supplierCode);
    Task<Supplier> GetByNameAsync(string supplierName);
    Task<bool> SupplierCodeExistsAsync(string supplierCode);
    Task<bool> SupplierNameExistsAsync(string supplierName, int? excludeId = null);
    Task<string> GetNextSupplierCodeAsync();
    Task<string> GetLastSupplierCodeAsync();
    Task ReassignSupplierCodesAsync();
}
