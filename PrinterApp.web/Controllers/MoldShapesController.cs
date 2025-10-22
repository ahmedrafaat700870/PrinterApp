using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrinterApp.Models.ViewModels;
using PrinterApp.Services.Interfaces;
using PrinterApp.Web.Helpers;

namespace PrinterApp.Web.Controllers;

[Authorize]
public class MoldShapesController : Controller
{
    private readonly IMoldShapeService _moldShapeService;
    private readonly IWebHostEnvironment _environment;

    public MoldShapesController(IMoldShapeService moldShapeService, IWebHostEnvironment environment)
    {
        _moldShapeService = moldShapeService;
        _environment = environment;
    }

    // GET: MoldShapes
    public async Task<IActionResult> Index(string searchTerm)
    {
        IEnumerable<MoldShapeViewModel> shapes;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            shapes = await _moldShapeService.SearchShapesAsync(searchTerm);
            ViewData["CurrentFilter"] = searchTerm;
        }
        else
        {
            shapes = await _moldShapeService.GetAllShapesAsync();
        }

        return View(shapes);
    }

    // GET: MoldShapes/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View(new MoldShapeViewModel());
    }

    // POST: MoldShapes/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MoldShapeViewModel model)
    {


        ModelState.Remove(nameof(MoldShapeViewModel.ShapeImagePath));
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Handle image upload
        string imagePath = null;
        if (model.ShapeImage != null)
        {
            var uploadResult = await FileUploadHelper.UploadFileAsync(
                model.ShapeImage,
                _environment.WebRootPath,
                "uploads/mold-shapes",
                new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" },
                5 * 1024 * 1024 // 5MB max
            );

            if (!uploadResult.Success)
            {
                ModelState.AddModelError(string.Empty, uploadResult.ErrorMessage);
                return View(model);
            }

            imagePath = uploadResult.FilePath;
        }

        var (success, errors) = await _moldShapeService.CreateShapeAsync(model, imagePath);

        if (success)
        {
            TempData["Success"] = "Mold shape created successfully";
            return RedirectToAction(nameof(Index));
        }

        // If creation failed and image was uploaded, delete it
        if (!string.IsNullOrEmpty(imagePath))
        {
            FileUploadHelper.DeleteFile(_environment.WebRootPath, imagePath);
        }

        foreach (var error in errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        return View(model);
    }

    // GET: MoldShapes/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var shape = await _moldShapeService.GetShapeByIdAsync(id);
        if (shape == null)
        {
            TempData["Error"] = "Mold shape not found";
            return RedirectToAction(nameof(Index));
        }

        return View(shape);
    }

    // POST: MoldShapes/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(MoldShapeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Get old image path
        var existingShape = await _moldShapeService.GetShapeByIdAsync(model.Id);
        string oldImagePath = existingShape?.ShapeImagePath;

        // Handle new image upload
        string newImagePath = null;
        if (model.ShapeImage != null)
        {
            var uploadResult = await FileUploadHelper.UploadFileAsync(
                model.ShapeImage,
                _environment.WebRootPath,
                "uploads/mold-shapes",
                new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" },
                5 * 1024 * 1024 // 5MB max
            );

            if (!uploadResult.Success)
            {
                ModelState.AddModelError(string.Empty, uploadResult.ErrorMessage);
                return View(model);
            }

            newImagePath = uploadResult.FilePath;
        }

        var (success, errors) = await _moldShapeService.UpdateShapeAsync(model, newImagePath);

        if (success)
        {
            // Delete old image if new one was uploaded
            if (!string.IsNullOrEmpty(newImagePath) && !string.IsNullOrEmpty(oldImagePath))
            {
                FileUploadHelper.DeleteFile(_environment.WebRootPath, oldImagePath);
            }

            TempData["Success"] = "Mold shape updated successfully";
            return RedirectToAction(nameof(Index));
        }

        // If update failed and new image was uploaded, delete it
        if (!string.IsNullOrEmpty(newImagePath))
        {
            FileUploadHelper.DeleteFile(_environment.WebRootPath, newImagePath);
        }

        foreach (var error in errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }

        return View(model);
    }

    // POST: MoldShapes/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var shape = await _moldShapeService.GetShapeByIdAsync(id);
        string imagePath = shape?.ShapeImagePath;

        var (success, errors) = await _moldShapeService.DeleteShapeAsync(id);

        if (success)
        {
            // Delete image file
            if (!string.IsNullOrEmpty(imagePath))
            {
                FileUploadHelper.DeleteFile(_environment.WebRootPath, imagePath);
            }

            TempData["Success"] = "Mold shape deleted successfully";
        }
        else
        {
            TempData["Error"] = string.Join(", ", errors);
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: MoldShapes/ToggleStatus/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var (success, errors) = await _moldShapeService.ToggleShapeStatusAsync(id);

        if (success)
        {
            TempData["Success"] = "Mold shape status updated successfully";
        }
        else
        {
            TempData["Error"] = string.Join(", ", errors);
        }

        return RedirectToAction(nameof(Index));
    }
}