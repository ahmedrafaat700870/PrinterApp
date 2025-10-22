using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrinterApp.Models.ViewModels;
using PrinterApp.Services.Interfaces;
using PrinterApp.Web.Helpers;

namespace PrinterApp.Web.Controllers
{
    [Authorize]
    public class RollDirectionsController : Controller
    {
        private readonly IRollDirectionService _rollDirectionService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RollDirectionsController(IRollDirectionService rollDirectionService, IWebHostEnvironment webHostEnvironment)
        {
            _rollDirectionService = rollDirectionService;
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize(Policy = "Permission.ROLEDIRECTION.View")]
        public async Task<IActionResult> Index(string searchTerm)
        {
            IEnumerable<RollDirectionViewModel> directions;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                directions = await _rollDirectionService.SearchDirectionsAsync(searchTerm);
                ViewData["CurrentFilter"] = searchTerm;
            }
            else
            {
                directions = await _rollDirectionService.GetAllDirectionsAsync();
            }

            return View(directions);
        }

        [Authorize(Policy = "Permission.ROLEDIRECTION.Create")]

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Permission.ROLEDIRECTION.Create")]

        public async Task<IActionResult> Create(RollDirectionViewModel model)
        {
            ModelState.Remove("DirectionImage");
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string imagePath = null;
            if (model.ImageFile != null)
            {
                //imagePath = await FileUploadHelper.SaveImageAsync(model.ImageFile, _webHostEnvironment);

                var uploadResult = await FileUploadHelper.UploadFileAsync(
                model.ImageFile,
                _webHostEnvironment.WebRootPath,
                "uploads/directions",
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

            var (success, errors) = await _rollDirectionService.CreateDirectionAsync(model, imagePath);

            if (success)
            {
                TempData["Success"] = "Roll direction created successfully";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "Permission.ROLEDIRECTION.Edit")]

        public async Task<IActionResult> Edit(int id)
        {
            var direction = await _rollDirectionService.GetDirectionByIdAsync(id);
            if (direction == null)
            {
                TempData["Error"] = "Roll direction not found";
                return RedirectToAction(nameof(Index));
            }

            return View(direction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Permission.ROLEDIRECTION.Edit")]

        public async Task<IActionResult> Edit(RollDirectionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Get old direction for image deletion
            var oldDirection = await _rollDirectionService.GetDirectionByIdAsync(model.Id);

            string imagePath = null;
            if (model.ImageFile != null)
            {
                // Delete old image
                if (!string.IsNullOrEmpty(oldDirection?.DirectionImage))
                {
                    await FileUploadHelper.DeleteImageAsync(oldDirection.DirectionImage, _webHostEnvironment);
                }

                // Save new image
                imagePath = await FileUploadHelper.SaveImageAsync(model.ImageFile, _webHostEnvironment);
            }

            var (success, errors) = await _rollDirectionService.UpdateDirectionAsync(model, imagePath);

            if (success)
            {
                TempData["Success"] = "Roll direction updated successfully";
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
        [Authorize(Policy = "Permission.ROLEDIRECTION.Delete")]

        public async Task<IActionResult> Delete(int id)
        {
            var direction = await _rollDirectionService.GetDirectionByIdAsync(id);

            var (success, errors) = await _rollDirectionService.DeleteDirectionAsync(id, direction?.DirectionImage);

            if (success)
            {
                // Delete image
                if (!string.IsNullOrEmpty(direction?.DirectionImage))
                {
                    await FileUploadHelper.DeleteImageAsync(direction.DirectionImage, _webHostEnvironment);
                }

                TempData["Success"] = "Roll direction deleted successfully";
            }
            else
            {
                TempData["Error"] = string.Join(", ", errors);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Permission.ROLEDIRECTION.Edit")]

        public async Task<IActionResult> ToggleStatus(int id)
        {
            var (success, errors) = await _rollDirectionService.ToggleDirectionStatusAsync(id);

            if (success)
            {
                TempData["Success"] = "Roll direction status updated successfully";
            }
            else
            {
                TempData["Error"] = string.Join(", ", errors);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}