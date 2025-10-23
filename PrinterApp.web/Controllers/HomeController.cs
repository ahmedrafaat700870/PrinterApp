using Microsoft.AspNetCore.Mvc;
using PrinterApp.Services.Interfaces;

namespace PrinterApp.Web.Controllers
{
    public class HomeController  : Controller
    {
        private readonly ILanguageService _languageService;
        public HomeController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        public IActionResult Index()
        {
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
