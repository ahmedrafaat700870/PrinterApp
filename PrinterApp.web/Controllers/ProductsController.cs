using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrinterApp.Models.ViewModels;
using PrinterApp.Services.Interfaces;

namespace PrinterApp.Web.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: Products
        public async Task<IActionResult> Index(string searchTerm)
        {
            IEnumerable<ProductViewModel> products;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                products = await _productService.SearchProductsAsync(searchTerm);
                ViewData["CurrentFilter"] = searchTerm;
            }
            else
            {
                products = await _productService.GetAllProductsAsync();
            }

            return View(products);
        }

        // GET: Products/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = await _productService.GetProductForCreateAsync();
            return View(model);
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await _productService.GetProductForCreateAsync();
                return View(model);
            }

            var (success, errors) = await _productService.CreateProductAsync(model);

            if (success)
            {
                TempData["Success"] = "Product created successfully";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            var modelWithData = await _productService.GetProductForCreateAsync();
            modelWithData.ProductName = model.ProductName;
            modelWithData.ProductCode = model.ProductCode;
            modelWithData.IsPrinted = model.IsPrinted;
            modelWithData.RawMaterialId = model.RawMaterialId;
            modelWithData.SupplierId = model.SupplierId;
            modelWithData.Description = model.Description;
            modelWithData.SelectedAdditionIds = model.SelectedAdditionIds;

            return View(modelWithData);
        }

        // GET: Products/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductForEditAsync(id);
            if (product == null)
            {
                TempData["Error"] = "Product not found";
                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var modelWithData = await _productService.GetProductForEditAsync(model.Id);
                if (modelWithData != null)
                {
                    modelWithData.ProductName = model.ProductName;
                    modelWithData.ProductCode = model.ProductCode;
                    modelWithData.IsPrinted = model.IsPrinted;
                    modelWithData.RawMaterialId = model.RawMaterialId;
                    modelWithData.SupplierId = model.SupplierId;
                    modelWithData.Description = model.Description;
                    modelWithData.SelectedAdditionIds = model.SelectedAdditionIds;
                    modelWithData.IsActive = model.IsActive;
                }
                return View(modelWithData);
            }

            var (success, errors) = await _productService.UpdateProductAsync(model);

            if (success)
            {
                TempData["Success"] = "Product updated successfully";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            var productWithData = await _productService.GetProductForEditAsync(model.Id);
            return View(productWithData);
        }

        // POST: Products/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, errors) = await _productService.DeleteProductAsync(id);

            if (success)
            {
                TempData["Success"] = "Product deleted successfully";
            }
            else
            {
                TempData["Error"] = string.Join(", ", errors);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Products/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var (success, errors) = await _productService.ToggleProductStatusAsync(id);

            if (success)
            {
                TempData["Success"] = "Product status updated successfully";
            }
            else
            {
                TempData["Error"] = string.Join(", ", errors);
            }

            return RedirectToAction(nameof(Index));
        }

        // API: Get Next Product Code
        [HttpGet]
        public async Task<IActionResult> GetNextProductCode(int supplierId)
        {
            var nextCode = await _productService.GetNextProductCodeAsync(supplierId);
            return Json(new { code = nextCode });
        }
    }
}