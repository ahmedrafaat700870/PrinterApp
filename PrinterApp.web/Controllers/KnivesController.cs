using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrinterApp.Models.ViewModels;
using PrinterApp.Services.Interfaces;
using PrinterApp.Web.Models;

namespace PrinterApp.Web.Controllers
{
    [Authorize]
    public class KnivesController : Controller
    {
        private readonly IKnifeService _knifeService;

        public KnivesController(IKnifeService knifeService)
        {
            _knifeService = knifeService;
        }

        // GET: Knives
        [Authorize(Policy = "Permission.KNIVE.View")]
        public async Task<IActionResult> Index(string searchTerm, int pageNumber = 1, int pageSize = 25)
        {
            IEnumerable<KnifeViewModel> knives;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                knives = await _knifeService.SearchKnivesAsync(searchTerm);
                ViewData["CurrentFilter"] = searchTerm;
            }
            else
            {
                knives = await _knifeService.GetAllKnivesAsync();
            }

            var paginatedKnives = PaginatedList<KnifeViewModel>.Create(knives, pageNumber, pageSize);
            ViewData["PageIndex"] = paginatedKnives.PageIndex;
            ViewData["TotalPages"] = paginatedKnives.TotalPages;
            ViewData["TotalCount"] = paginatedKnives.TotalCount;
            ViewData["PageSize"] = paginatedKnives.PageSize;
            ViewData["HasPreviousPage"] = paginatedKnives.HasPreviousPage;
            ViewData["HasNextPage"] = paginatedKnives.HasNextPage;

            return View(paginatedKnives);
        }

        // GET: Knives/Create
        [Authorize(Policy = "Permission.KNIVE.Create")]

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Knives/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Permission.KNIVE.Create")]
        public async Task<IActionResult> Create(KnifeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, errors) = await _knifeService.CreateKnifeAsync(model);

            if (success)
            {
                TempData["Success"] = "Knife created successfully";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(model);
        }

        // GET: Knives/Edit/5
        [Authorize(Policy = "Permission.KNIVE.Edit")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var knife = await _knifeService.GetKnifeByIdAsync(id);
            if (knife == null)
            {
                TempData["Error"] = "Knife not found";
                return RedirectToAction(nameof(Index));
            }

            return View(knife);
        }

        // POST: Knives/Edit/5
        [Authorize(Policy = "Permission.KNIVE.Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(KnifeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, errors) = await _knifeService.UpdateKnifeAsync(model);

            if (success)
            {
                TempData["Success"] = "Knife updated successfully";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(model);
        }

        // POST: Knives/Delete/5
        [Authorize(Policy = "Permission.KNIVE.Delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, errors) = await _knifeService.DeleteKnifeAsync(id);

            if (success)
            {
                TempData["Success"] = "Knife deleted successfully";
            }
            else
            {
                TempData["Error"] = string.Join(", ", errors);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Knives/ToggleStatus/5
        [Authorize(Policy = "Permission.KNIVE.Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var (success, errors) = await _knifeService.ToggleKnifeStatusAsync(id);

            if (success)
            {
                TempData["Success"] = "Knife status updated successfully";
            }
            else
            {
                TempData["Error"] = string.Join(", ", errors);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}