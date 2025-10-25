using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrinterApp.Models.ViewModels;
using PrinterApp.Services.Interfaces;
using PrinterApp.Web.Models;

namespace PrinterApp.Web.Controllers;

[Authorize] // يتطلب تسجيل الدخول
public class CoresController : Controller
{
    private readonly ICoreService _coreService;

    public CoresController(ICoreService coreService)
    {
        _coreService = coreService;
    }

    // GET: Cores

    [Authorize(Policy = "Permission.CORE.View")]
    public async Task<IActionResult> Index(string searchTerm, int pageNumber = 1, int pageSize = 25)
    {
        IEnumerable<CoreViewModel> cores;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            cores = await _coreService.SearchCoresAsync(searchTerm);
            ViewData["CurrentFilter"] = searchTerm;
        }
        else
        {
            cores = await _coreService.GetAllCoresAsync();
        }

        var paginatedCores = PaginatedList<CoreViewModel>.Create(cores, pageNumber, pageSize);
        ViewData["PageIndex"] = paginatedCores.PageIndex;
        ViewData["TotalPages"] = paginatedCores.TotalPages;
        ViewData["TotalCount"] = paginatedCores.TotalCount;
        ViewData["PageSize"] = paginatedCores.PageSize;
        ViewData["HasPreviousPage"] = paginatedCores.HasPreviousPage;
        ViewData["HasNextPage"] = paginatedCores.HasNextPage;

        return View(paginatedCores);
    }

    // POST: Cores/Search (للـ AJAX)
    [Authorize(Policy = "Permission.CORE.View")]
    [HttpPost]
    public async Task<IActionResult> Search([FromBody] SearchRequest request)
    {
        var cores = await _coreService.SearchCoresAsync(request.SearchTerm);
        return PartialView("_CoresTablePartial", cores);
    }

    // POST: Cores/Create
    [Authorize(Policy = "Permission.CORE.Create")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CoreViewModel model)
    {

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var (success , errors) = await _coreService.CreateCoreAsync(model);

        if(!success)
        {
            foreach(var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
            return View(model);
        }

        TempData["Success"] = "Core created successfully";

        return RedirectToAction(nameof(Index));


    }

    // GET: Cores/Edit/5
    [Authorize(Policy = "Permission.CORE.Edit")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var core = await _coreService.GetCoreByIdAsync(id);
        if (core == null)
        {
            TempData["Error"] = "Core not found";
            return RedirectToAction(nameof(Index));
        }

        return View(core);
    }

    // POST: Cores/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "Permission.CORE.Edit")]
    public async Task<IActionResult> Edit(CoreViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var (success, errors) = await _coreService.UpdateCoreAsync(model);

        if (success)
        {
            TempData["Success"] = "Core updated successfully";
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        return View(model);
    }

    // POST: Cores/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "Permission.CORE.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, errors) = await _coreService.DeleteCoreAsync(id);

        if (success)
        {
            TempData["Success"] = "Core deleted successfully";
        }
        else
        {
            TempData["Error"] = string.Join(", ", errors);
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: Cores/ToggleStatus/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "Permission.CORE.Edit")]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var (success, errors) = await _coreService.ToggleCoreStatusAsync(id);

        if (success)
        {
            TempData["Success"] = "Core status updated successfully";
        }
        else
        {
            TempData["Error"] = string.Join(", ", errors);
        }

        return RedirectToAction(nameof(Index));
    }
}