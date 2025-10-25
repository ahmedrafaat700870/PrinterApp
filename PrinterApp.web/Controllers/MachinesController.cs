using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrinterApp.Models.ViewModels;
using PrinterApp.Services.Interfaces;
using PrinterApp.Web.Models;

namespace PrinterApp.Web.Controllers
{
    [Authorize]
    public class MachinesController : Controller
    {
        private readonly IMachineService _machineService;

        public MachinesController(IMachineService machineService)
        {
            _machineService = machineService;
        }

        // GET: Machines
        [Authorize(Policy = "Permission.MACHINE.View")]
        public async Task<IActionResult> Index(string searchTerm, int pageNumber = 1, int pageSize = 25)
        {
            IEnumerable<MachineViewModel> machines;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                machines = await _machineService.SearchMachinesAsync(searchTerm);
                ViewData["CurrentFilter"] = searchTerm;
            }
            else
            {
                machines = await _machineService.GetAllMachinesAsync();
            }

            var paginatedMachines = PaginatedList<MachineViewModel>.Create(machines, pageNumber, pageSize);
            ViewData["PageIndex"] = paginatedMachines.PageIndex;
            ViewData["TotalPages"] = paginatedMachines.TotalPages;
            ViewData["TotalCount"] = paginatedMachines.TotalCount;
            ViewData["PageSize"] = paginatedMachines.PageSize;
            ViewData["HasPreviousPage"] = paginatedMachines.HasPreviousPage;
            ViewData["HasNextPage"] = paginatedMachines.HasNextPage;

            return View(paginatedMachines);
        }

        // GET: Machines/Create
        [HttpGet]
        [Authorize(Policy = "Permission.MACHINE.Create")]

        public IActionResult Create()
        {
            return View();
        }

        // POST: Machines/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Permission.MACHINE.Create")]

        public async Task<IActionResult> Create(MachineViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, errors) = await _machineService.CreateMachineAsync(model);

            if (success)
            {
                TempData["Success"] = "Machine created successfully";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(model);
        }

        // GET: Machines/Edit/5
        [HttpGet]
        [Authorize(Policy = "Permission.MACHINE.Edit")]

        public async Task<IActionResult> Edit(int id)
        {
            var machine = await _machineService.GetMachineByIdAsync(id);
            if (machine == null)
            {
                TempData["Error"] = "Machine not found";
                return RedirectToAction(nameof(Index));
            }

            return View(machine);
        }

        // POST: Machines/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Permission.MACHINE.Edit")]

        public async Task<IActionResult> Edit(MachineViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, errors) = await _machineService.UpdateMachineAsync(model);

            if (success)
            {
                TempData["Success"] = "Machine updated successfully";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(model);
        }

        // POST: Machines/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Permission.MACHINE.Delete")]

        public async Task<IActionResult> Delete(int id)
        {
            var (success, errors) = await _machineService.DeleteMachineAsync(id);

            if (success)
            {
                TempData["Success"] = "Machine deleted successfully";
            }
            else
            {
                TempData["Error"] = string.Join(", ", errors);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Machines/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Permission.MACHINE.Edit")]

        public async Task<IActionResult> ToggleStatus(int id)
        {
            var (success, errors) = await _machineService.ToggleMachineStatusAsync(id);

            if (success)
            {
                TempData["Success"] = "Machine status updated successfully";
            }
            else
            {
                TempData["Error"] = string.Join(", ", errors);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}