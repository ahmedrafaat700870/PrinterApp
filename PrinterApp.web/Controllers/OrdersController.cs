using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PrinterApp.Data.UnitOfWork;
using PrinterApp.Models.Entities;
using PrinterApp.Models.ViewModels;
using PrinterApp.Services.Interfaces;

namespace PrinterApp.Web.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(
            IOrderService orderService,
            ICustomerService customerService,
            IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager)
        {
            _orderService = orderService;
            _customerService = customerService;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        // GET: Orders
        public async Task<IActionResult> Index(string searchTerm, string status, string stage)
        {
            IEnumerable<OrderViewModel> orders;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                orders = await _orderService.SearchOrdersAsync(searchTerm);
            }
            else if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<OrderStatus>(status, out var orderStatus))
            {
                orders = await _orderService.GetOrdersByStatusAsync(orderStatus);
            }
            else if (!string.IsNullOrWhiteSpace(stage) && Enum.TryParse<OrderStage>(stage, out var orderStage))
            {
                orders = await _orderService.GetOrdersByStageAsync(orderStage);
            }
            else
            {
                orders = await _orderService.GetActiveOrdersAsync();
            }

            ViewData["CurrentFilter"] = searchTerm;
            ViewData["CurrentStatus"] = status;
            ViewData["CurrentStage"] = stage;

            return View(orders);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var orderDetails = await _orderService.GetOrderWithAllDetailsAsync(id);
            if (orderDetails == null)
            {
                TempData["Error"] = "الطلب غير موجود";
                return RedirectToAction(nameof(Index));
            }

            return View(orderDetails);
        }

        // GET: Orders/Create - Stage 1
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCreateDropdowns();
            return View(new OrderStage1ViewModel { OrderDate = DateTime.Today, ExpectedDeliveryDate = DateTime.Today.AddDays(7) });
        }

        // POST: Orders/Create - Stage 1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderStage1ViewModel model, IFormFileCollection files)
        {
            RemoveModelStateKeys();

            if (!ModelState.IsValid)
            {
                await LoadCreateDropdowns();
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            var (success, errors) = await _orderService.CreateOrderStage1Async(model, files, user.Id, user.FullName);

            if (success)
            {
                TempData["Success"] = "تم إنشاء الطلب بنجاح";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            await LoadCreateDropdowns();
            return View(model);
        }

        // GET: Orders/Edit/5 - Stage 1
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var orderEntity = await _unitOfWork.Orders.GetByIdAsync(id);
            if (orderEntity == null)
            {
                TempData["Error"] = "الطلب غير موجود";
                return RedirectToAction(nameof(Index));
            }

            if (orderEntity.Stage != OrderStage.Order)
            {
                TempData["Error"] = "لا يمكن تعديل الطلب في هذه المرحلة";
                return RedirectToAction(nameof(Details), new { id });
            }

            var model = new OrderStage1ViewModel
            {
                Id = orderEntity.Id,
                OrderNumber = orderEntity.OrderNumber,
                OrderDate = orderEntity.OrderDate,
                ExpectedDeliveryDate = orderEntity.ExpectedDeliveryDate,
                CustomerId = orderEntity.CustomerId,
                SupplierId = orderEntity.SupplierId,
                ProductId = orderEntity.ProductId,
                RollDirectionId = orderEntity.RollDirectionId,
                RawMaterialId = orderEntity.RawMaterialId,
                Length = orderEntity.Length,
                Width = orderEntity.Width,
                Quantity = orderEntity.Quantity,
                OrderNotes = orderEntity.OrderNotes ?? string.Empty
            };

            await LoadCreateDropdowns();
            return View(model);
        }

        // POST: Orders/Edit/5 - Stage 1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OrderStage1ViewModel model, IFormFileCollection files)
        {
            RemoveModelStateKeys();

            if (!ModelState.IsValid)
            {
                await LoadCreateDropdowns();
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            var (success, errors) = await _orderService.UpdateOrderStage1Async(model, files, user.Id, user.FullName);

            if (success)
            {
                TempData["Success"] = "تم تحديث الطلب بنجاح";
                return RedirectToAction(nameof(Details), new { id = model.Id });
            }

            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            await LoadCreateDropdowns();
            return View(model);
        }

        // POST: Orders/MoveToReview/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MoveToReview(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var (success, errors) = await _orderService.MoveToReviewAsync(id, user.Id, user.FullName);

            if (success)
            {
                TempData["Success"] = "تم نقل الطلب إلى مرحلة المراجعة";
            }
            else
            {
                TempData["Error"] = string.Join(", ", errors);
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Orders/ReviewList - Stage 2 List
        public async Task<IActionResult> ReviewList()
        {
            var orders = await _orderService.GetReviewOrdersAsync();
            return View(orders);
        }

        // GET: Orders/Review/5 - Stage 2
        [HttpGet]
        public async Task<IActionResult> Review(int id)
        {
            var model = await _orderService.GetOrderForReviewAsync(id);
            if (model == null)
            {
                TempData["Error"] = "الطلب غير موجود";
                return RedirectToAction(nameof(ReviewList));
            }

            await LoadReviewDropdowns();
            return View(model);
        }

        // POST: Orders/Review/5 - Stage 2
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Review(OrderStage2ViewModel model)
        {
            RemoveStage2ModelStateKeys();

            if (!ModelState.IsValid)
            {
                await LoadReviewDropdowns();
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            var (success, errors) = await _orderService.UpdateOrderStage2Async(model, user.Id, user.FullName);

            if (success)
            {
                TempData["Success"] = "تم مراجعة الطلب بنجاح";
                return RedirectToAction(nameof(ReviewList));
            }

            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            await LoadReviewDropdowns();
            return View(model);
        }

        // POST: Orders/MoveToManufacturing/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MoveToManufacturing(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var (success, errors) = await _orderService.MoveToManufacturingAsync(id, user.Id, user.FullName);

            if (success)
            {
                TempData["Success"] = "تم نقل الطلب إلى مرحلة التصنيع";
            }
            else
            {
                TempData["Error"] = string.Join(", ", errors);
            }

            return RedirectToAction(nameof(ReviewList));
        }

        // GET: Orders/ManufacturingList - Stage 3 List
        public async Task<IActionResult> ManufacturingList()
        {
            var orders = await _orderService.GetManufacturingOrdersAsync();
            return View(orders);
        }

        // GET: Orders/Manufacturing/5 - Stage 3
        [HttpGet]
        public async Task<IActionResult> Manufacturing(int id)
        {
            var model = await _orderService.GetOrderForManufacturingAsync(id);
            if (model == null)
            {
                TempData["Error"] = "الطلب غير موجود";
                return RedirectToAction(nameof(ManufacturingList));
            }

            return View(model);
        }

        // POST: Orders/CompleteManufacturingItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteManufacturingItem(int orderId, int itemId)
        {
            var user = await _userManager.GetUserAsync(User);
            var (success, errors) = await _orderService.CompleteManufacturingItemAsync(orderId, itemId, user.Id, user.FullName);

            if (success)
            {
                TempData["Success"] = "تم إكمال العنصر بنجاح";
            }
            else
            {
                TempData["Error"] = string.Join(", ", errors);
            }

            return RedirectToAction(nameof(Manufacturing), new { id = orderId });
        }

        // POST: Orders/MoveToPrinting/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MoveToPrinting(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var (success, errors) = await _orderService.MoveToPrintingAsync(id, user.Id, user.FullName);

            if (success)
            {
                TempData["Success"] = "تم نقل الطلب إلى مرحلة الطباعة";
            }
            else
            {
                TempData["Error"] = string.Join(", ", errors);
            }

            return RedirectToAction(nameof(ManufacturingList));
        }

        // GET: Orders/PrintingList - Stage 4 List
        public async Task<IActionResult> PrintingList()
        {
            var orders = await _orderService.GetPrintingOrdersAsync();
            return View(orders);
        }

        // GET: Orders/Printing/5 - Stage 4
        [HttpGet]
        public async Task<IActionResult> Printing(int id)
        {
            var model = await _orderService.GetOrderForPrintingAsync(id);
            if (model == null)
            {
                TempData["Error"] = "الطلب غير موجود";
                return RedirectToAction(nameof(PrintingList));
            }

            return View(model);
        }

        // POST: Orders/StartPrinting/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartPrinting(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var (success, errors) = await _orderService.StartPrintingAsync(id, user.Id, user.FullName);

            if (success)
            {
                TempData["Success"] = "تم بدء الطباعة";
            }
            else
            {
                TempData["Error"] = string.Join(", ", errors);
            }

            return RedirectToAction(nameof(Printing), new { id });
        }

        // POST: Orders/CompletePrinting/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompletePrinting(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var (success, errors) = await _orderService.CompletePrintingAsync(id, user.Id, user.FullName);

            if (success)
            {
                TempData["Success"] = "تم إكمال الطلب بنجاح";
                return RedirectToAction(nameof(PrintingList));
            }

            TempData["Error"] = string.Join(", ", errors);
            return RedirectToAction(nameof(Printing), new { id });
        }

        // POST: Orders/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id, string reason)
        {
            var user = await _userManager.GetUserAsync(User);
            var (success, errors) = await _orderService.CancelOrderAsync(id, reason, user.Id, user.FullName);

            if (success)
            {
                TempData["Success"] = "تم إلغاء الطلب";
            }
            else
            {
                TempData["Error"] = string.Join(", ", errors);
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Orders/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, errors) = await _orderService.DeleteOrderAsync(id);

            if (success)
            {
                TempData["Success"] = "تم حذف الطلب بنجاح";
            }
            else
            {
                TempData["Error"] = string.Join(", ", errors);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Orders/Statistics
        public async Task<IActionResult> Statistics()
        {
            var statistics = await _orderService.GetOrderStatisticsAsync();
            return View(statistics);
        }

        // API Methods
        [HttpGet]
        public async Task<IActionResult> CheckOrderNumber(string orderNumber, int? excludeId)
        {
            var exists = await _orderService.OrderNumberExistsAsync(orderNumber, excludeId);
            return Json(new { exists });
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsBySupplier(int supplierId)
        {
            var products = await _unitOfWork.Products.GetProductsBySupplierAsync(supplierId);
            return Json(products.Select(p => new { id = p.Id, name = p.ProductName }));
        }

        // Helper Methods
        private async Task LoadCreateDropdowns()
        {
            var customers = await _unitOfWork.Customers.GetAllAsync();
            var suppliers = await _unitOfWork.Suppliers.GetAllAsync();
            var products = await _unitOfWork.Products.GetAllAsync();
            var rollDirections = await _unitOfWork.RollDirections.GetAllAsync();
            var rawMaterials = await _unitOfWork.RawMaterials.GetAllAsync();

            ViewBag.Customers = new SelectList(customers.Where(c => c.IsActive), "Id", "CustomerName");
            ViewBag.Suppliers = new SelectList(suppliers.Where(s => s.IsActive), "Id", "SupplierName");
            ViewBag.Products = new SelectList(products.Where(p => p.IsActive), "Id", "ProductName");
            ViewBag.RollDirections = new SelectList(rollDirections.Where(r => r.IsActive), "Id", "DirectionNumber");
            ViewBag.RawMaterials = new SelectList(rawMaterials.Where(r => r.IsActive), "Id", "RawMaterialName");
        }

        private async Task LoadReviewDropdowns()
        {
            var machines = await _unitOfWork.Machines.GetAllAsync();
            var cores = await _unitOfWork.Cores.GetAllAsync();
            var knives = await _unitOfWork.Knives.GetAllAsync();
            var cartons = await _unitOfWork.Cartons.GetAllAsync();
            var molds = await _unitOfWork.Molds.GetAllAsync();
            var manufacturingAdditions = await _unitOfWork.ManufacturingAdditions.GetAllAsync();

            ViewBag.Machines = new SelectList(machines.Where(m => m.IsActive), "Id", "MachineName");
            ViewBag.Cores = new SelectList(cores.Where(c => c.IsActive), "Id", "CoreName");
            ViewBag.Knives = new SelectList(knives.Where(k => k.IsActive), "Id", "KnifeName");
            ViewBag.Cartons = new SelectList(cartons.Where(c => c.IsActive), "Id", "CartonName");
            ViewBag.Molds = new SelectList(molds.Where(m => m.IsActive), "Id", "MoldNumber");
            ViewBag.ManufacturingAdditions = manufacturingAdditions.Where(m => m.IsActive).ToList();
        }

        private void RemoveModelStateKeys()
        {
            ModelState.Remove("CustomerName");
            ModelState.Remove("SupplierName");
            ModelState.Remove("ProductName");
            ModelState.Remove("RawMaterialName");
        }

        private void RemoveStage2ModelStateKeys()
        {
            ModelState.Remove("OrderNumber");
            ModelState.Remove("CustomerName");
            ModelState.Remove("ProductName");
            ModelState.Remove("RawMaterialName");
        }
    }
}
