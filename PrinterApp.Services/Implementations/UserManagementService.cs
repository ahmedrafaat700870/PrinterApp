using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrinterApp.Models.Entities;
using PrinterApp.Models.ViewModels;
using PrinterApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrinterApp.Services.Implementations;

public class UserManagementService : IUserManagementService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserManagementService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<List<UserRoleViewModel>> GetAllUsersAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var userRoles = new List<UserRoleViewModel>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userRoles.Add(new UserRoleViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = roles.FirstOrDefault() ?? "User",
                AvailableRoles = _roleManager.Roles.Select(r => r.Name).ToList()
            });
        }

        return userRoles;
    }

    public async Task<UserRoleViewModel> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);

        return new UserRoleViewModel
        {
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Role = roles.FirstOrDefault() ?? "User",
            AvailableRoles = _roleManager.Roles.Select(r => r.Name).ToList()
        };
    }

    public async Task<(bool Success, string[] Errors)> AssignRoleAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return (false, new[] { "User not found" });
        }

        // إزالة جميع الأدوار الحالية
        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);

        // إضافة الدور الجديد
        var result = await _userManager.AddToRoleAsync(user, roleName);

        if (result.Succeeded)
        {
            return (true, null);
        }

        return (false, result.Errors.Select(e => e.Description).ToArray());
    }

    public async Task<(bool Success, string[] Errors)> RemoveFromRoleAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return (false, new[] { "User not found" });
        }

        var result = await _userManager.RemoveFromRoleAsync(user, roleName);

        if (result.Succeeded)
        {
            return (true, null);
        }

        return (false, result.Errors.Select(e => e.Description).ToArray());
    }

    public async Task<List<string>> GetUserRolesAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return new List<string>();

        var roles = await _userManager.GetRolesAsync(user);
        return roles.ToList();
    }
}