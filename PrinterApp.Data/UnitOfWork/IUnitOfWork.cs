

using PrinterApp.Data.Repositories;

namespace PrinterApp.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {

        IPermissionRepository Permissions { get; }
        IPermissionRoleRepository PermissionRoles { get; }
        IUserPermissionRepository UserPermissions { get; }
        ISystemSettingRepository  SystemSettings { get; }
        ICoreRepository Cores { get; }
        IRollDirectionRepository RollDirections { get; }
        IMachineRepository Machines { get; }
        ICartonRepository Cartons { get; }
        IKnifeRepository Knives { get; }
        ISupplierRepository Suppliers { get; }
        IRawMaterialRepository RawMaterials { get; }
        IManufacturingAdditionRepository ManufacturingAdditions { get; }
        IProductRepository Products { get; } 
        IProductAdditionRepository ProductAdditions { get; }
        IMoldShapeRepository MoldShapes { get; } 
        IMoldRepository Molds { get; } 
        Task<int> CompleteAsync();
    }
}
