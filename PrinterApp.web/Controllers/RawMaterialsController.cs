using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrinterApp.Models.ViewModels;
using PrinterApp.Services.Interfaces;
using PrinterApp.Web.Models;

namespace PrinterApp.Web.Controllers
{
    [Authorize]
    public class RawMaterialsController : Controller
    {
        private readonly IRawMaterialService _rawMaterialService;

        public RawMaterialsController(IRawMaterialService rawMaterialService)
        {
            _rawMaterialService = rawMaterialService;
        }

        // GET: RawMaterials
        [Authorize(Policy = "Permission.RAWMATERIALS.View")]
        public async Task<IActionResult> Index(string searchTerm, int pageNumber = 1, int pageSize = 25)
        {
            IEnumerable<RawMaterialViewModel> rawMaterials;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                rawMaterials = await _rawMaterialService.SearchRawMaterialsAsync(searchTerm);
                ViewData["CurrentFilter"] = searchTerm;
            }
            else
            {
                rawMaterials = await _rawMaterialService.GetAllRawMaterialsAsync();
            }

            var paginatedRawMaterials = PaginatedList<RawMaterialViewModel>.Create(rawMaterials, pageNumber, pageSize);
            ViewData["PageIndex"] = paginatedRawMaterials.PageIndex;
            ViewData["TotalPages"] = paginatedRawMaterials.TotalPages;
            ViewData["TotalCount"] = paginatedRawMaterials.TotalCount;
            ViewData["PageSize"] = paginatedRawMaterials.PageSize;
            ViewData["HasPreviousPage"] = paginatedRawMaterials.HasPreviousPage;
            ViewData["HasNextPage"] = paginatedRawMaterials.HasNextPage;

            return View(paginatedRawMaterials);
        }

        // GET: RawMaterials/Create
        [Authorize(Policy = "Permission.RAWMATERIALS.Create")]

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: RawMaterials/Create
        [HttpPost]
        [Authorize(Policy = "Permission.RAWMATERIALS.Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RawMaterialViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, errors) = await _rawMaterialService.CreateRawMaterialAsync(model);

            if (success)
            {
                TempData["Success"] = "Raw material created successfully";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(model);
        }

        // GET: RawMaterials/Edit/5
        [Authorize(Policy = "Permission.RAWMATERIALS.Edit")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var rawMaterial = await _rawMaterialService.GetRawMaterialByIdAsync(id);
            if (rawMaterial == null)
            {
                TempData["Error"] = "Raw material not found";
                return RedirectToAction(nameof(Index));
            }

            return View(rawMaterial);
        }

        // POST: RawMaterials/Edit/5
        [HttpPost]
        [Authorize(Policy = "Permission.RAWMATERIALS.Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RawMaterialViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, errors) = await _rawMaterialService.UpdateRawMaterialAsync(model);

            if (success)
            {
                TempData["Success"] = "Raw material updated successfully";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(model);
        }

        // POST: RawMaterials/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Permission.RAWMATERIALS.Delete")]

        public async Task<IActionResult> Delete(int id)
        {
            var (success, errors) = await _rawMaterialService.DeleteRawMaterialAsync(id);

            if (success)
            {
                TempData["Success"] = "Raw material deleted successfully";
            }
            else
            {
                TempData["Error"] = string.Join(", ", errors);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: RawMaterials/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Permission.RAWMATERIALS.Edit")]

        public async Task<IActionResult> ToggleStatus(int id)
        {
            var (success, errors) = await _rawMaterialService.ToggleRawMaterialStatusAsync(id);

            if (success)
            {
                TempData["Success"] = "Raw material status updated successfully";
            }
            else
            {
                TempData["Error"] = string.Join(", ", errors);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}