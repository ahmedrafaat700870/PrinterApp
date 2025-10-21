

using PrinterApp.Data.Repositories;

namespace PrinterApp.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {

        IPermissionRepository Permissions { get; }
        IPermissionRoleRepository PermissionRoles { get; }
        IUserPermissionRepository UserPermissions { get; }
        ISystemSettingRepository  SystemSettings { get; }
        Task<int> CompleteAsync();
    }
}
