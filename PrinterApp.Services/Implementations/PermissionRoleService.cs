
using PrinterApp.Data.UnitOfWork;
using PrinterApp.Models.Entities;
using PrinterApp.Models.ViewModels;
using PrinterApp.Services.Interfaces;

namespace PrinterApp.Services.Implementations;

public class PermissionRoleService : IPermissionRoleService
{
    private readonly IUnitOfWork _unitOfWork;

    public PermissionRoleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<PermissionRoleViewModel>> GetRolesByPermissionIdAsync(int permissionId)
    {
        var roles = await _unitOfWork.PermissionRoles.GetRolesByPermissionIdAsync(permissionId);

        return roles.Select(r => new PermissionRoleViewModel
        {
            Id = r.Id,
            PermissionId = r.PermissionId,
            RoleName = r.RoleName,
            Description = r.Description,
            CreatedDate = r.CreatedDate
        }).ToList();
    }

    public async Task<(bool Success, string[] Errors)> CreateRoleAsync(int permissionId, PermissionRoleViewModel model)
    {
        try
        {
            var existingRole = await _unitOfWork.PermissionRoles
                .GetByPermissionAndRoleNameAsync(permissionId, model.RoleName);

            if (existingRole != null)
            {
                return (false, new[] { "Role already exists for this permission" });
            }

            var role = new PermissionRole
            {
                PermissionId = permissionId,
                RoleName = model.RoleName,
                Description = model.Description,
                CreatedDate = DateTime.Now
            };

            await _unitOfWork.PermissionRoles.AddAsync(role);
            await _unitOfWork.CompleteAsync();

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, new[] { ex.Message });
        }
    }

    public async Task<(bool Success, string[] Errors)> DeleteRoleAsync(int roleId)
    {
        try
        {
            var role = await _unitOfWork.PermissionRoles.GetByIdAsync(roleId);
            if (role == null)
            {
                return (false, new[] { "Role not found" });
            }

            _unitOfWork.PermissionRoles.Delete(role);
            await _unitOfWork.CompleteAsync();

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, new[] { ex.Message });
        }
    }
}