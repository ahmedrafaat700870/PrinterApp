using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PrinterApp.Models.Entities;
using PrinterApp.Models.ViewModels;
using PrinterApp.Services.Interfaces;
using PrinterApp.Web.Models;

namespace PrinterApp.Web.Controllers
{
    [Authorize]
    public class PrintOrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;

        public PrintOrdersController(
            IOrderService orderService,
            UserManager<ApplicationUser> userManager)
        {
            _orderService = orderService;
            _userManager = userManager;
        }

        // GET: PrintOrders
        public async Task<IActionResult> Index(string searchTerm, string status, string stage, DateTime? fromDate, DateTime? toDate, int pageNumber = 1, int pageSize = 25)
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
                orders = (await _orderService.GetAllOrdersAsync()).Where(x =>x.IsActive);
            }

            // Apply date range filter
            if (fromDate.HasValue)
            {
                orders = orders.Where(o => o.OrderDate.Date >= fromDate.Value.Date);
            }

            if (toDate.HasValue)
            {
                orders = orders.Where(o => o.OrderDate.Date <= toDate.Value.Date);
            }

            // Apply pagination
            var paginatedOrders = PaginatedList<OrderViewModel>.Create(orders, pageNumber, pageSize);

            ViewData["CurrentFilter"] = searchTerm;
            ViewData["CurrentStatus"] = status;
            ViewData["CurrentStage"] = stage;
            ViewData["FromDate"] = fromDate?.ToString("yyyy-MM-dd");
            ViewData["ToDate"] = toDate?.ToString("yyyy-MM-dd");
            
            // Pagination data
            ViewData["PageIndex"] = paginatedOrders.PageIndex;
            ViewData["TotalPages"] = paginatedOrders.TotalPages;
            ViewData["TotalCount"] = paginatedOrders.TotalCount;
            ViewData["PageSize"] = paginatedOrders.PageSize;
            ViewData["HasPreviousPage"] = paginatedOrders.HasPreviousPage;
            ViewData["HasNextPage"] = paginatedOrders.HasNextPage;

            return View(paginatedOrders);
        }

        // GET: PrintOrders/PrintQueue - Manager Only
        [Authorize(Policy = "Permission.ORDERS.Manage")]
        public async Task<IActionResult> PrintQueue(string searchTerm, DateTime? fromDate, DateTime? toDate, int pageNumber = 1, int pageSize = 25)
        {
            var printQueue = await _orderService.GetPrintQueueOrderedByPriorityAsync();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                printQueue = printQueue.Where(o =>
                    o.OrderNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (o.CustomerName != null && o.CustomerName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (o.ProductName != null && o.ProductName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }

            // Apply date range filter
            if (fromDate.HasValue)
            {
                printQueue = printQueue.Where(o => o.ExpectedDeliveryDate.Date >= fromDate.Value.Date);
            }

            if (toDate.HasValue)
            {
                printQueue = printQueue.Where(o => o.ExpectedDeliveryDate.Date <= toDate.Value.Date);
            }

            // Apply pagination
            var paginatedQueue = PaginatedList<PrintQueueViewModel>.Create(printQueue, pageNumber, pageSize);

            ViewData["CurrentFilter"] = searchTerm;
            ViewData["FromDate"] = fromDate?.ToString("yyyy-MM-dd");
            ViewData["ToDate"] = toDate?.ToString("yyyy-MM-dd");
            
            // Pagination data
            ViewData["PageIndex"] = paginatedQueue.PageIndex;
            ViewData["TotalPages"] = paginatedQueue.TotalPages;
            ViewData["TotalCount"] = paginatedQueue.TotalCount;
            ViewData["PageSize"] = paginatedQueue.PageSize;
            ViewData["HasPreviousPage"] = paginatedQueue.HasPreviousPage;
            ViewData["HasNextPage"] = paginatedQueue.HasNextPage;

            return View(paginatedQueue);
        }

        // GET: PrintOrders/PrintDetails/5
        public async Task<IActionResult> PrintDetails(int id)
        {
            var order = await _orderService.GetOrderWithAllDetailsAsync(id);
            if (order == null)
            {
                TempData["Error"] = "الطلب غير موجود";
                return RedirectToAction(nameof(Index));
            }

            return View(order);
        }

        // POST: PrintOrders/UpdatePriority
        [HttpPost]
        [Authorize(Policy = "Permission.ORDERS.Manage")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePriority(int orderId, int priority)
        {
            var user = await _userManager.GetUserAsync(User);
            var (success, errors) = await _orderService.UpdatePrintOrderPriorityAsync(orderId, priority, user.Id, user.FullName);

            if (success)
            {
                TempData["Success"] = "تم تحديث الأولوية بنجاح";
            }
            else
            {
                TempData["Error"] = string.Join(", ", errors);
            }

            return RedirectToAction(nameof(PrintQueue));
        }

        // POST: PrintOrders/UpdatePriorityAjax (for AJAX calls)
        [HttpPost]
        [Authorize(Policy = "Permission.ORDERS.Manage")]
        public async Task<IActionResult> UpdatePriorityAjax(int orderId, int priority)
        {
            var user = await _userManager.GetUserAsync(User);
            var (success, errors) = await _orderService.UpdatePrintOrderPriorityAsync(orderId, priority, user.Id, user.FullName);

            if (success)
            {
                return Json(new { success = true, message = "تم تحديث الأولوية بنجاح" });
            }

            return Json(new { success = false, message = string.Join(", ", errors) });
        }

        // POST: PrintOrders/ReorderQueue
        [HttpPost]
        [Authorize(Policy = "Permission.ORDERS.Manage")]
        public async Task<IActionResult> ReorderQueue([FromBody] Dictionary<int, int> orderPriorities)
        {
            if (orderPriorities == null || !orderPriorities.Any())
            {
                return Json(new { success = false, message = "لا توجد بيانات للترتيب" });
            }

            var user = await _userManager.GetUserAsync(User);
            var (success, errors) = await _orderService.ReorderPrintQueueAsync(orderPriorities, user.Id, user.FullName);

            if (success)
            {
                return Json(new { success = true, message = "تم إعادة ترتيب القائمة بنجاح" });
            }

            return Json(new { success = false, message = string.Join(", ", errors) });
        }

        // GET: PrintOrders/GetPrintQueueItem/5 (AJAX)
        [HttpGet]
        public async Task<IActionResult> GetPrintQueueItem(int id)
        {
            var item = await _orderService.GetPrintQueueItemAsync(id);
            if (item == null)
            {
                return Json(new { success = false, message = "الطلب غير موجود" });
            }

            return Json(new { success = true, data = item });
        }

        // GET: PrintOrders/Edit/5
        [HttpGet]
        [Authorize(Policy = "Permission.ORDERS.Manage")]
        public async Task<IActionResult> Edit(int id)
        {
            // Redirect to Orders controller Edit action
            return RedirectToAction("Edit", "Orders", new { id = id });
        }

        // POST: PrintOrders/Delete/5
        [HttpPost]
        [Authorize(Policy = "Permission.ORDERS.Manage")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var order = await _orderService.GetOrderByIdAsync(id);

            if (order == null)
            {
                TempData["Error"] = "الطلب غير موجود";
                return RedirectToAction(nameof(Index));
            }

            var (success, errors) = await _orderService.DeleteOrderAsync(id);

            if (success)
            {
                TempData["Success"] = $"تم حذف الطلب رقم {order.OrderNumber} بنجاح";
            }
            else
            {
                TempData["Error"] = string.Join(", ", errors);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
