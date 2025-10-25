using Microsoft.AspNetCore.Mvc;
using PrinterApp.Services.Interfaces;
using PrinterApp.Models.Entities;

namespace PrinterApp.Web.Controllers
{
    public class HomeController  : Controller
    {
        private readonly ILanguageService _languageService;
        private readonly IOrderService _orderService;
        
        public HomeController(ILanguageService languageService, IOrderService orderService)
        {
            _languageService = languageService;
            _orderService = orderService;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                // Get statistics
                var allOrders = await _orderService.GetActiveOrdersAsync();
                
                ViewBag.TotalOrders = allOrders.Count();
                ViewBag.PendingOrders = allOrders.Count(o => o.Status == OrderStatus.Pending);
                ViewBag.InPrintingOrders = allOrders.Count(o => o.Stage == OrderStage.Printing);
                ViewBag.CompletedOrders = allOrders.Count(o => o.Status == OrderStatus.Completed);
                ViewBag.LateOrders = allOrders.Count(o => o.IsLate);
                
                // Print queue statistics
                var printQueue = await _orderService.GetPrintQueueOrderedByPriorityAsync();
                ViewBag.PrintQueueCount = printQueue.Count();
                ViewBag.HighPriorityCount = printQueue.Count(o => o.Priority <= 5);
            }
            
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetLanguage(string language, string returnUrl)
        {
            // Validate language
            if (string.IsNullOrEmpty(language) || (language != "ar" && language != "en"))
            {
                language = "ar"; // Default to Arabic
            }

            // Set language in session
            _languageService.SetLanguage(language);

            // Redirect to return URL or home
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Index));
        }

       

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }

    }
}
