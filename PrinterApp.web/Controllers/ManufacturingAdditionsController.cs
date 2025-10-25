using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrinterApp.Models.ViewModels;
using PrinterApp.Services.Interfaces;
using PrinterApp.Web.Models;

namespace PrinterApp.Web.Controllers
{
    [Authorize]
    public class ManufacturingAdditionsController : Controller
    {
        private readonly IManufacturingAdditionService _additionService;

        public ManufacturingAdditionsController(IManufacturingAdditionService additionService)
        {
            _additionService = additionService;
        }

        // GET: ManufacturingAdditions
        [Authorize(Policy = "Permission.MANUFACTURINGADDITIONS.View")]
        public async Task<IActionResult> Index(string searchTerm, int pageNumber = 1, int pageSize = 25)
        {
            IEnumerable<ManufacturingAdditionViewModel> additions;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                additions = await _additionService.SearchAdditionsAsync(searchTerm);
                ViewData["CurrentFilter"] = searchTerm;
            }
            else
            {
                additions = await _additionService.GetAllAdditionsAsync();
            }

            var paginatedAdditions = PaginatedList<ManufacturingAdditionViewModel>.Create(additions, pageNumber, pageSize);
            ViewData["PageIndex"] = paginatedAdditions.PageIndex;
            ViewData["TotalPages"] = paginatedAdditions.TotalPages;
            ViewData["TotalCount"] = paginatedAdditions.TotalCount;
            ViewData["PageSize"] = paginatedAdditions.PageSize;
            ViewData["HasPreviousPage"] = paginatedAdditions.HasPreviousPage;
            ViewData["HasNextPage"] = paginatedAdditions.HasNextPage;

            return View(paginatedAdditions);
        }

        // GET: ManufacturingAdditions/Create
        [HttpGet]
        [Authorize(Policy = "Permission.MANUFACTURINGADDITIONS.Create")]

        public IActionResult Create()
        {
            return View();
        }

        // POST: ManufacturingAdditions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Permission.MANUFACTURINGADDITIONS.Create")]

        public async Task<IActionResult> Create(ManufacturingAdditionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, errors) = await _additionService.CreateAdditionAsync(model);

            if (success)
            {
                TempData["Success"] = "Manufacturing addition created successfully";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(model);
        }

        // GET: ManufacturingAdditions/Edit/5
        [HttpGet]
        [Authorize(Policy = "Permission.MANUFACTURINGADDITIONS.Edit")]

        public async Task<IActionResult> Edit(int id)
        {
            var addition = await _additionService.GetAdditionByIdAsync(id);
            if (addition == null)
            {
                TempData["Error"] = "Addition not found";
                return RedirectToAction(nameof(Index));
            }

            return View(addition);
        }

        // POST: ManufacturingAdditions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Permission.MANUFACTURINGADDITIONS.Edit")]

        public async Task<IActionResult> Edit(ManufacturingAdditionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, errors) = await _additionService.UpdateAdditionAsync(model);

            if (success)
            {
                TempData["Success"] = "Manufacturing addition updated successfully";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(model);
        }

        // POST: ManufacturingAdditions/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Permission.MANUFACTURINGADDITIONS.Delete")]

        public async Task<IActionResult> Delete(int id)
        {
            var (success, errors) = await _additionService.DeleteAdditionAsync(id);

            if (success)
            {
                TempData["Success"] = "Manufacturing addition deleted successfully";
            }
            else
            {
                TempData["Error"] = string.Join(", ", errors);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: ManufacturingAdditions/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Permission.MANUFACTURINGADDITIONS.Edit")]

        public async Task<IActionResult> ToggleStatus(int id)
        {
            var (success, errors) = await _additionService.ToggleAdditionStatusAsync(id);

            if (success)
            {
                TempData["Success"] = "Addition status updated successfully";
            }
            else
            {
                TempData["Error"] = string.Join(", ", errors);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}