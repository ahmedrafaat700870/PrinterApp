using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrinterApp.Data;
using PrinterApp.Data.UnitOfWork;
using PrinterApp.Models.Entities;
using PrinterApp.Models.ViewModels;
using PrinterApp.Services.Interfaces;
namespace PrinterApp.Services.Implementations;

public class UserPermissionService : IUserPermissionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public UserPermissionService(
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _context = context;
    }

    public async Task<UserPermissionsViewModel> GetUserPermissionsAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return null;

        var userPermissions = await _unitOfWork.UserPermissions.GetUserPermissionsAsync(userId);
        var allPermissions = await _context.Permissions
            .Include(p => p.PermissionRoles)
            .ToListAsync();

        var viewModel = new UserPermissionsViewModel
        {
            UserId = userId,
            Email = user.Email,
            FullName = user.FullName,
            Permissions = allPermissions.Select(p => new PermissionWithRolesViewModel
            {
                PermissionId = p.Id,
                PermissionName = p.Name,
                PermissionCode = p.Code,
                Description = p.Description,
                Roles = p.PermissionRoles.Select(r => new RoleCheckViewModel
                {
                    PermissionRoleId = r.Id,
                    RoleName = r.RoleName,
                    Description = r.Description,
                    IsGranted = userPermissions.Any(up =>
                        up.PermissionId == p.Id && up.PermissionRoleId == r.Id)
                }).ToList()
            }).ToList()
        };

        return viewModel;
    }

    public async Task<(bool Success, string[] Errors)> UpdateUserPermissionsAsync(
        string userId,
        Dictionary<int, List<int>> permissionRoles)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return (false, new[] { "User not found" });
            }

            // حذف جميع الصلاحيات الحالية للمستخدم
            var existingPermissions = await _unitOfWork.UserPermissions.GetUserPermissionsAsync(userId);
            foreach (var up in existingPermissions)
            {
                _unitOfWork.UserPermissions.Delete(up);
            }

            // إضافة الصلاحيات الجديدة
            if (permissionRoles != null && permissionRoles.Any())
            {
                foreach (var permission in permissionRoles)
                {
                    int permissionId = permission.Key;
                    List<int> roleIds = permission.Value;

                    foreach (var roleId in roleIds)
                    {
                        await _unitOfWork.UserPermissions.GrantPermissionRoleAsync(
                            userId, permissionId, roleId);
                    }
                }
            }

            await _unitOfWork.CompleteAsync();
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, new[] { ex.Message });
        }
    }

    public async Task<bool> UserHasPermissionRoleAsync(string userId, string permissionCode, string roleName)
    {
        return await _unitOfWork.UserPermissions
            .UserHasPermissionRoleAsync(userId, permissionCode, roleName);
    }
}