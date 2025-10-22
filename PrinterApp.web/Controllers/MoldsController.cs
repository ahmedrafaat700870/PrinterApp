using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrinterApp.Models.ViewModels;
using PrinterApp.Services.Interfaces;

namespace PrinterApp.Web.Controllers;

[Authorize]
public class MoldsController : Controller
{
    private readonly IMoldService _moldService;

    public MoldsController(IMoldService moldService)
    {
        _moldService = moldService;
    }

    // GET: Molds
    public async Task<IActionResult> Index(string searchTerm)
    {
        IEnumerable<MoldViewModel> molds;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            molds = await _moldService.SearchMoldsAsync(searchTerm);
            ViewData["CurrentFilter"] = searchTerm;
        }
        else
        {
            molds = await _moldService.GetAllMoldsAsync();
        }

        return View(molds);
    }

    // GET: Molds/Create
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = await _moldService.GetMoldForCreateAsync();
        return View(model);
    }

    // POST: Molds/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MoldViewModel model)
    {
        ModelState.Remove(nameof(MoldViewModel.Machines));
        ModelState.Remove(nameof(MoldViewModel.MoldShapes));
        ModelState.Remove(nameof(MoldViewModel.MachineCode));
        ModelState.Remove(nameof(MoldViewModel.MachineName));
        ModelState.Remove(nameof(MoldViewModel.MoldShapeName));
        ModelState.Remove(nameof(MoldViewModel.ShapeImagePath));
        if (!ModelState.IsValid)
        {
            var modelWithData = await _moldService.GetMoldForCreateAsync();
            modelWithData.MoldNumber = model.MoldNumber;
            modelWithData.MachineId = model.MachineId;
            modelWithData.MoldShapeId = model.MoldShapeId;
            modelWithData.Width = model.Width;
            modelWithData.Height = model.Height;
            modelWithData.PrintedRawMaterialSize = model.PrintedRawMaterialSize;
            modelWithData.PlainRawMaterialSize = model.PlainRawMaterialSize;
            modelWithData.Description = model.Description;
            return View(modelWithData);
        }

        var (success, errors) = await _moldService.CreateMoldAsync(model);

        if (success)
        {
            TempData["Success"] = "Mold created successfully";
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        var modelData = await _moldService.GetMoldForCreateAsync();
        modelData.MoldNumber = model.MoldNumber;
        modelData.MachineId = model.MachineId;
        modelData.MoldShapeId = model.MoldShapeId;
        modelData.Width = model.Width;
        modelData.Height = model.Height;
        modelData.PrintedRawMaterialSize = model.PrintedRawMaterialSize;
        modelData.PlainRawMaterialSize = model.PlainRawMaterialSize;
        modelData.Description = model.Description;

        return View(modelData);
    }

    // GET: Molds/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var mold = await _moldService.GetMoldForEditAsync(id);
        if (mold == null)
        {
            TempData["Error"] = "Mold not found";
            return RedirectToAction(nameof(Index));
        }

        return View(mold);
    }

    // POST: Molds/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(MoldViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var modelWithData = await _moldService.GetMoldForEditAsync(model.Id);
            if (modelWithData != null)
            {
                modelWithData.MoldNumber = model.MoldNumber;
                modelWithData.MachineId = model.MachineId;
                modelWithData.MoldShapeId = model.MoldShapeId;
                modelWithData.Width = model.Width;
                modelWithData.Height = model.Height;
                modelWithData.PrintedRawMaterialSize = model.PrintedRawMaterialSize;
                modelWithData.PlainRawMaterialSize = model.PlainRawMaterialSize;
                modelWithData.Description = model.Description;
                modelWithData.IsActive = model.IsActive;
            }
            return View(modelWithData);
        }

        var (success, errors) = await _moldService.UpdateMoldAsync(model);

        if (success)
        {
            TempData["Success"] = "Mold updated successfully";
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        var moldWithData = await _moldService.GetMoldForEditAsync(model.Id);
        return View(moldWithData);
    }

    // POST: Molds/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, errors) = await _moldService.DeleteMoldAsync(id);

        if (success)
        {
            TempData["Success"] = "Mold deleted successfully";
        }
        else
        {
            TempData["Error"] = string.Join(", ", errors);
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: Molds/ToggleStatus/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var (success, errors) = await _moldService.ToggleMoldStatusAsync(id);

        if (success)
        {
            TempData["Success"] = "Mold status updated successfully";
        }
        else
        {
            TempData["Error"] = string.Join(", ", errors);
        }

        return RedirectToAction(nameof(Index));
    }

    // API: Calculate Total Eyes
    [HttpGet]
    public IActionResult CalculateTotalEyes(int width, int height)
    {
        var totalEyes = _moldService.CalculateTotalEyes(width, height);
        return Json(new { totalEyes });
    }
}