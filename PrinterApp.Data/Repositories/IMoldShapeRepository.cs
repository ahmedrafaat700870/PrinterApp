using PrinterApp.Models.Entities;

namespace PrinterApp.Data.Repositories
{
    public interface IMoldShapeRepository : IRepository<MoldShape>
    {
        Task<List<MoldShape>> GetActiveShapesAsync();
        Task<MoldShape> GetByNameAsync(string shapeName);
        Task<bool> ShapeNameExistsAsync(string shapeName, int? excludeId = null);
        Task<MoldShape> GetShapeWithMoldsAsync(int id);
    }
}