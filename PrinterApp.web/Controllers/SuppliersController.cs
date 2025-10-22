using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrinterApp.Models.ViewModels;
using PrinterApp.Services.Interfaces;

namespace PrinterApp.Web.Controllers;

[Authorize]
public class SuppliersController : Controller
{
    private readonly ISupplierService _supplierService;

    public SuppliersController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    // GET: Suppliers
    [Authorize(Policy = "Permission.SUPPLIER.View")]
    public async Task<IActionResult> Index(string searchTerm)
    {
        IEnumerable<SupplierViewModel> suppliers;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            suppliers = await _supplierService.SearchSuppliersAsync(searchTerm);
            ViewData["CurrentFilter"] = searchTerm;
        }
        else
        {
            suppliers = await _supplierService.GetAllSuppliersAsync();
        }

        return View(suppliers);
    }

    // GET: Suppliers/Create
    [HttpGet]
    [Authorize(Policy = "Permission.SUPPLIER.Create")]
    public async Task<IActionResult> Create()
    {
        var nextCode = await _supplierService.GetNextSupplierCodeAsync();
        ViewBag.NextSupplierCode = nextCode;
        return View();
    }

    // POST: Suppliers/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "Permission.SUPPLIER.Create")]

    public async Task<IActionResult> Create(SupplierViewModel model)
    {
        ModelState.Remove(nameof(SupplierViewModel.SupplierCode));
        if (!ModelState.IsValid)
        {
            var nextCode = await _supplierService.GetNextSupplierCodeAsync();
            ViewBag.NextSupplierCode = nextCode;
            return View(model);
        }

        var (success, errors) = await _supplierService.CreateSupplierAsync(model);

        if (success)
        {
            TempData["Success"] = "Supplier created successfully";
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        var code = await _supplierService.GetNextSupplierCodeAsync();
        ViewBag.NextSupplierCode = code;
        return View(model);
    }

    // GET: Suppliers/Edit/5
    [HttpGet]
    [Authorize(Policy = "Permission.SUPPLIER.Edit")]

    public async Task<IActionResult> Edit(int id)
    {
        var supplier = await _supplierService.GetSupplierByIdAsync(id);
        if (supplier == null)
        {
            TempData["Error"] = "Supplier not found";
            return RedirectToAction(nameof(Index));
        }

        return View(supplier);
    }

    // POST: Suppliers/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "Permission.SUPPLIER.Edit")]

    public async Task<IActionResult> Edit(SupplierViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var (success, errors) = await _supplierService.UpdateSupplierAsync(model);

        if (success)
        {
            TempData["Success"] = "Supplier updated successfully";
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        return View(model);
    }

    // POST: Suppliers/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "Permission.SUPPLIER.Delete")]

    public async Task<IActionResult> Delete(int id)
    {
        var (success, errors) = await _supplierService.DeleteSupplierAsync(id);

        if (success)
        {
            TempData["Success"] = "Supplier deleted successfully and codes reassigned";
        }
        else
        {
            TempData["Error"] = string.Join(", ", errors);
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: Suppliers/ToggleStatus/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "Permission.SUPPLIER.Edit")]

    public async Task<IActionResult> ToggleStatus(int id)
    {
        var (success, errors) = await _supplierService.ToggleSupplierStatusAsync(id);

        if (success)
        {
            TempData["Success"] = "Supplier status updated successfully";
        }
        else
        {
            TempData["Error"] = string.Join(", ", errors);
        }

        return RedirectToAction(nameof(Index));
    }
}