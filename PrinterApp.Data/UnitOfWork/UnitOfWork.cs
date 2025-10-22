using PrinterApp.Data.Repositories;

namespace PrinterApp.Data.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IPermissionRepository _permissions;
    private IPermissionRoleRepository _permissionRoles;
    private IUserPermissionRepository _userPermissions;
    private ISystemSettingRepository _systemRepository;
    private ICoreRepository _coreRepository;
    private IRollDirectionRepository _rollDirectionRepository;
    private IMachineRepository _machines;
    private ICartonRepository _cartons;
    private IKnifeRepository _knives; 
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }
    public ISystemSettingRepository SystemSettings
    {
        get
        {
            if (_systemRepository == null)
            {
                _systemRepository = new SystemSettingRepository(_context);
            }
            return _systemRepository;
        }
    }
    public IPermissionRepository Permissions
    {
        get
        {
            if (_permissions == null)
            {
                _permissions = new PermissionRepository(_context);
            }
            return _permissions;
        }
    }
    public IPermissionRoleRepository PermissionRoles
    {
        get
        {
            if (_permissionRoles == null)
            {
                _permissionRoles = new PermissionRoleRepository(_context);
            }
            return _permissionRoles;
        }
    }
    public ICoreRepository Cores
    {
        get
        {
            if (_coreRepository is null)
            {
                _coreRepository = new CoreRepository(_context);
            }
            return _coreRepository;
        }
    }
    public IUserPermissionRepository UserPermissions
    {
        get
        {
            if (_userPermissions == null)
            {
                _userPermissions = new UserPermissionRepository(_context);
            }
            return _userPermissions;
        }
    }
    public IRollDirectionRepository RollDirections
    {
        get
        {
            if (_rollDirectionRepository == null)
            {
                _rollDirectionRepository = new RollDirectionRepository(_context);
            }
            return _rollDirectionRepository;
        }
    }
    public IMachineRepository Machines
    {
        get
        {
            if (_machines == null)
            {
                _machines = new MachineRepository(_context);
            }
            return _machines;
        }
    }
    public ICartonRepository Cartons
    {
        get
        {
            if (_cartons == null)
            {
                _cartons = new CartonRepository(_context);
            }
            return _cartons;
        }
    }

    public IKnifeRepository Knives
    {
        get
        {
            if (_knives is null)
            {
                _knives = new   KnifeRepository(_context);
            }
            return _knives;
        }
    }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }
    public void Dispose()
    {
        _context.Dispose();
    }
}