using Microsoft.AspNetCore.Mvc;
using PrinterApp.Models.ViewModels;
using PrinterApp.Services.Interfaces;

namespace PrinterApp.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // قائمة العملاء
        [HttpGet]
        public async Task<IActionResult> Index(string search = null)
        {
            IEnumerable<CustomerViewModel> customers;

            if (!string.IsNullOrEmpty(search))
            {
                customers = await _customerService.SearchCustomersAsync(search);
            }
            else
            {
                customers = await _customerService.GetAllCustomersAsync();
            }

            ViewBag.Search = search;
            return View(customers);
        }

        // تفاصيل العميل
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var customer = await _customerService.GetCustomerWithOrdersAsync(id);

            if (customer == null)
            {
                TempData["ErrorMessage"] = "العميل غير موجود";
                return RedirectToAction(nameof(Index));
            }

            return View(customer);
        }

        // إضافة عميل
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.Identity.Name ?? "System";

            var result = await _customerService.CreateCustomerAsync(model, userId);

            if (result.Success)
            {
                TempData["SuccessMessage"] = "تم إضافة العميل بنجاح";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(model);
        }

        // تعديل عميل
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);

            if (customer == null)
            {
                TempData["ErrorMessage"] = "العميل غير موجود";
                return RedirectToAction(nameof(Index));
            }

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CustomerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.Identity.Name ?? "System";

            var result = await _customerService.UpdateCustomerAsync(model, userId);

            if (result.Success)
            {
                TempData["SuccessMessage"] = "تم تحديث العميل بنجاح";
                return RedirectToAction(nameof(Details), new { id = model.Id });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View(model);
        }

        // حذف عميل
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _customerService.DeleteCustomerAsync(id);

            if (result.Success)
            {
                TempData["SuccessMessage"] = "تم حذف العميل بنجاح";
            }
            else
            {
                TempData["ErrorMessage"] = string.Join(", ", result.Errors);
            }

            return RedirectToAction(nameof(Index));
        }

        // تفعيل/تعطيل العميل
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var result = await _customerService.ToggleCustomerStatusAsync(id);

            if (result.Success)
            {
                TempData["SuccessMessage"] = "تم تغيير حالة العميل بنجاح";
            }
            else
            {
                TempData["ErrorMessage"] = string.Join(", ", result.Errors);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}