using PrinterApp.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrinterApp.Data.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IPermissionRepository _permissions;
    private IPermissionRoleRepository _permissionRoles;
    private IUserPermissionRepository _userPermissions;
    private ISystemSettingRepository _systemRepository;

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

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}