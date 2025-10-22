using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrinterApp.Models.ViewModels;
using PrinterApp.Services.Interfaces;

namespace PrinterApp.Web.Controllers
{
    [Authorize]
    public class CartonsController : Controller
    {
        private readonly ICartonService _cartonService;

        public CartonsController(ICartonService cartonService)
        {
            _cartonService = cartonService;
        }

        // GET: Cartons
        [Authorize(Policy = "Permission.CARTON.View")]
        public async Task<IActionResult> Index(string searchTerm)
        {
            IEnumerable<CartonViewModel> cartons;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                cartons = await _cartonService.SearchCartonsAsync(searchTerm);
                ViewData["CurrentFilter"] = searchTerm;
            }
            else
            {
                cartons = await _cartonService.GetAllCartonsAsync();
            }

            return View(cartons);
        }

        // GET: Cartons/Create
        [Authorize(Policy = "Permission.CARTON.Create")]

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cartons/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Permission.CARTON.Create")]

        public async Task<IActionResult> Create(CartonViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, errors) = await _cartonService.CreateCartonAsync(model);

            if (success)
            {
                TempData["Success"] = "Carton created successfully";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(model);
        }

        // GET: Cartons/Edit/5
        [HttpGet]
        [Authorize(Policy = "Permission.CARTON.Edit")]

        public async Task<IActionResult> Edit(int id)
        {
            var carton = await _cartonService.GetCartonByIdAsync(id);
            if (carton == null)
            {
                TempData["Error"] = "Carton not found";
                return RedirectToAction(nameof(Index));
            }

            return View(carton);
        }

        // POST: Cartons/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Permission.CARTON.Edit")]

        public async Task<IActionResult> Edit(CartonViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, errors) = await _cartonService.UpdateCartonAsync(model);

            if (success)
            {
                TempData["Success"] = "Carton updated successfully";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(model);
        }

        // POST: Cartons/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Permission.CARTON.Delete")]

        public async Task<IActionResult> Delete(int id)
        {
            var (success, errors) = await _cartonService.DeleteCartonAsync(id);

            if (success)
            {
                TempData["Success"] = "Carton deleted successfully";
            }
            else
            {
                TempData["Error"] = string.Join(", ", errors);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Cartons/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Permission.CARTON.Edit")]

        public async Task<IActionResult> ToggleStatus(int id)
        {
            var (success, errors) = await _cartonService.ToggleCartonStatusAsync(id);

            if (success)
            {
                TempData["Success"] = "Carton status updated successfully";
            }
            else
            {
                TempData["Error"] = string.Join(", ", errors);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}