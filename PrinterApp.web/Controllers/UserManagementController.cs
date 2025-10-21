using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrinterApp.Services.Interfaces;

namespace PrinterApp.Web.Controllers;

[Authorize(Policy = "Permission.USERS.Manage")]
public class UserManagementController : Controller
{
    private readonly IUserManagementService _userManagementService;
    private readonly IUserPermissionService _userPermissionService;

    public UserManagementController(
        IUserManagementService userManagementService,
        IUserPermissionService userPermissionService)
    {
        _userManagementService = userManagementService;
        _userPermissionService = userPermissionService;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userManagementService.GetAllUsersAsync();
        return View(users);
    }

    [HttpGet]
    public async Task<IActionResult> AssignRole(string userId)
    {
        var user = await _userManagementService.GetUserByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }
        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignRole(string userId, string role)
    {
        var (success, errors) = await _userManagementService.AssignRoleAsync(userId, role);

        if (success)
        {
            TempData["Success"] = "Role assigned successfully";
            return RedirectToAction(nameof(Index));
        }

        TempData["Error"] = string.Join(", ", errors);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> ManagePermissions(string userId)
    {
        var userPermissions = await _userPermissionService.GetUserPermissionsAsync(userId);
        if (userPermissions == null)
        {
            return NotFound();
        }
        return View(userPermissions);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ManagePermissions(string userId, Dictionary<int, List<int>> permissionRoles)
    {
        var (success, errors) = await _userPermissionService
            .UpdateUserPermissionsAsync(userId, permissionRoles);

        if (success)
        {
            TempData["Success"] = "Permissions updated successfully";
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        var userPermissions = await _userPermissionService.GetUserPermissionsAsync(userId);
        return View(userPermissions);
    }
}
