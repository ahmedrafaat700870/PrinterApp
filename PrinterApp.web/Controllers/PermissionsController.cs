using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrinterApp.Models.ViewModels;
using PrinterApp.Services.Interfaces;

namespace PrinterApp.Web.Controllers;

[Authorize(Policy = "Permission.PERMISSIONS.Manage")]
public class PermissionsController : Controller
{
    private readonly IPermissionService _permissionService;
    private readonly IPermissionRoleService _permissionRoleService;

    public PermissionsController(
        IPermissionService permissionService,
        IPermissionRoleService permissionRoleService)
    {
        _permissionService = permissionService;
        _permissionRoleService = permissionRoleService;
    }

    public async Task<IActionResult> Index()
    {
        var permissions = await _permissionService.GetAllPermissionsAsync();
        return View(permissions);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PermissionViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var (success, errors) = await _permissionService.CreatePermissionAsync(model);

        if (success)
        {
            TempData["Success"] = "Permission created successfully";
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var permission = await _permissionService.GetPermissionByIdAsync(id);
        if (permission == null)
        {
            return NotFound();
        }
        return View(permission);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(PermissionViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var (success, errors) = await _permissionService.UpdatePermissionAsync(model);

        if (success)
        {
            TempData["Success"] = "Permission updated successfully";
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _permissionService.DeletePermissionAsync(id);

        if (result)
        {
            TempData["Success"] = "Permission deleted successfully";
        }
        else
        {
            TempData["Error"] = "Permission not found";
        }

        return RedirectToAction(nameof(Index));
    }

    // إدارة الأدوار داخل الصلاحية
    [HttpGet]
    public async Task<IActionResult> ManageRoles(int id)
    {
        var permission = await _permissionService.GetPermissionWithRolesAsync(id);
        if (permission == null)
        {
            return NotFound();
        }
        return View(permission);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddRole(int permissionId, PermissionRoleViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Invalid role data";
            return RedirectToAction(nameof(ManageRoles), new { id = permissionId });
        }

        var (success, errors) = await _permissionRoleService.CreateRoleAsync(permissionId, model);

        if (success)
        {
            TempData["Success"] = "Role added successfully";
        }
        else
        {
            TempData["Error"] = string.Join(", ", errors);
        }

        return RedirectToAction(nameof(ManageRoles), new { id = permissionId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteRole(int roleId, int permissionId)
    {
        var (success, errors) = await _permissionRoleService.DeleteRoleAsync(roleId);

        if (success)
        {
            TempData["Success"] = "Role deleted successfully";
        }
        else
        {
            TempData["Error"] = string.Join(", ", errors);
        }

        return RedirectToAction(nameof(ManageRoles), new { id = permissionId });
    }
}