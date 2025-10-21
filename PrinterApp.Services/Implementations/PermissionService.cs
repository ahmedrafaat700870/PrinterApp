using Microsoft.EntityFrameworkCore;
using PrinterApp.Data;
using PrinterApp.Data.UnitOfWork;
using PrinterApp.Models.Entities;
using PrinterApp.Models.ViewModels;
using PrinterApp.Services.Interfaces;
namespace PrinterApp.Services.Implementations;

public class PermissionService : IPermissionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;

    public PermissionService(IUnitOfWork unitOfWork, ApplicationDbContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<IEnumerable<PermissionViewModel>> GetAllPermissionsAsync()
    {
        var permissions = await _unitOfWork.Permissions.GetAllAsync();
        return permissions.Select(p => new PermissionViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Code = p.Code,
            CreatedDate = p.CreatedDate
        });
    }

    public async Task<PermissionViewModel> GetPermissionByIdAsync(int id)
    {
        var permission = await _unitOfWork.Permissions.GetByIdAsync(id);
        if (permission == null) return null;

        return new PermissionViewModel
        {
            Id = permission.Id,
            Name = permission.Name,
            Description = permission.Description,
            Code = permission.Code,
            CreatedDate = permission.CreatedDate
        };
    }

    public async Task<PermissionViewModel> GetPermissionWithRolesAsync(int id)
    {
        var permission = await _context.Permissions
            .Include(p => p.PermissionRoles)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (permission == null) return null;

        return new PermissionViewModel
        {
            Id = permission.Id,
            Name = permission.Name,
            Description = permission.Description,
            Code = permission.Code,
            CreatedDate = permission.CreatedDate,
            Roles = permission.PermissionRoles.Select(r => new PermissionRoleViewModel
            {
                Id = r.Id,
                PermissionId = r.PermissionId,
                RoleName = r.RoleName,
                Description = r.Description,
                CreatedDate = r.CreatedDate
            }).ToList()
        };
    }

    public async Task<(bool Success, string[] Errors)> CreatePermissionAsync(PermissionViewModel model)
    {
        var exists = await _unitOfWork.Permissions.ExistsAsync(model.Name);
        if (exists)
        {
            return (false, new[] { "Permission with this name already exists" });
        }

        var permission = new Permission
        {
            Name = model.Name,
            Description = model.Description,
            Code = model.Code,
            CreatedDate = DateTime.Now
        };

        await _unitOfWork.Permissions.AddAsync(permission);
        await _unitOfWork.CompleteAsync();

        return (true, null);
    }

    public async Task<(bool Success, string[] Errors)> UpdatePermissionAsync(PermissionViewModel model)
    {
        var permission = await _unitOfWork.Permissions.GetByIdAsync(model.Id);
        if (permission == null)
        {
            return (false, new[] { "Permission not found" });
        }

        permission.Name = model.Name;
        permission.Description = model.Description;
        permission.Code = model.Code;

        _unitOfWork.Permissions.Update(permission);
        await _unitOfWork.CompleteAsync();

        return (true, null);
    }

    public async Task<bool> DeletePermissionAsync(int id)
    {
        var permission = await _unitOfWork.Permissions.GetByIdAsync(id);
        if (permission == null) return false;

        _unitOfWork.Permissions.Delete(permission);
        await _unitOfWork.CompleteAsync();

        return true;
    }
}
