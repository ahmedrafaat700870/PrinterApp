using Microsoft.AspNetCore.Mvc.Rendering;
using PrinterApp.Data.UnitOfWork;
using PrinterApp.Models.Entities;
using PrinterApp.Models.ViewModels;
using PrinterApp.Services.Interfaces;

namespace PrinterApp.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ProductViewModel>> GetAllProductsAsync()
        {
            var products = await _unitOfWork.Products.GetProductsWithDetailsAsync();
            return products.Select(MapToViewModel);
        }

        public async Task<IEnumerable<ProductViewModel>> GetActiveProductsAsync()
        {
            var products = await _unitOfWork.Products.GetActiveProductsAsync();
            return products.Select(MapToViewModel);
        }

        public async Task<IEnumerable<ProductViewModel>> SearchProductsAsync(string searchTerm)
        {
            var products = await _unitOfWork.Products.GetProductsWithDetailsAsync();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return products.Select(MapToViewModel);
            }

            searchTerm = searchTerm.ToLower().Trim();

            var filteredProducts = products.Where(p =>
                p.ProductName.ToLower().Contains(searchTerm) ||
                p.ProductCode.ToLower().Contains(searchTerm) ||
                p.Supplier.SupplierName.ToLower().Contains(searchTerm) ||
                p.RawMaterial.RawMaterialName.ToLower().Contains(searchTerm)
            );

            return filteredProducts.Select(MapToViewModel);
        }

        public async Task<ProductViewModel> GetProductByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetProductWithDetailsAsync(id);
            return product != null ? MapToViewModel(product) : null;
        }

        public async Task<ProductViewModel> GetProductForCreateAsync()
        {
            var model = new ProductViewModel();
            await PopulateDropdownsAsync(model);
            return model;
        }

        public async Task<ProductViewModel> GetProductForEditAsync(int id)
        {
            var product = await _unitOfWork.Products.GetProductWithDetailsAsync(id);
            if (product == null) return null;

            var model = MapToViewModel(product);
            await PopulateDropdownsAsync(model);

            // Set selected additions
            model.SelectedAdditionIds = product.ProductAdditions
                .Select(pa => pa.ManufacturingAdditionId)
                .ToList();

            // Mark selected additions in checkbox list
            foreach (var addition in model.AvailableAdditions)
            {
                addition.IsSelected = model.SelectedAdditionIds.Contains(addition.Id);
            }

            return model;
        }

        public async Task<(bool Success, string[] Errors)> CreateProductAsync(ProductViewModel model)
        {
            try
            {
                // Validate product code
                if (await _unitOfWork.Products.ProductCodeExistsAsync(model.ProductCode))
                {
                    return (false, new[] { "A product with this code already exists" });
                }

                // Validate product name
                if (await _unitOfWork.Products.ProductNameExistsAsync(model.ProductName))
                {
                    return (false, new[] { "A product with this name already exists" });
                }

                // Validate supplier exists
                var supplier = await _unitOfWork.Suppliers.GetByIdAsync(model.SupplierId);
                if (supplier == null)
                {
                    return (false, new[] { "Selected supplier not found" });
                }

                // Validate raw material exists
                var rawMaterial = await _unitOfWork.RawMaterials.GetByIdAsync(model.RawMaterialId);
                if (rawMaterial == null)
                {
                    return (false, new[] { "Selected raw material not found" });
                }

                // Validate product code starts with supplier code
                if (!model.ProductCode.StartsWith(supplier.SupplierCode))
                {
                    return (false, new[] { $"Product code must start with supplier code: {supplier.SupplierCode}" });
                }

                var product = new Product
                {
                    ProductName = model.ProductName,
                    ProductCode = model.ProductCode,
                    IsPrinted = model.IsPrinted,
                    RawMaterialId = model.RawMaterialId,
                    SupplierId = model.SupplierId,
                    Description = model.Description,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };

                await _unitOfWork.Products.AddAsync(product);
                await _unitOfWork.CompleteAsync();

                // Add selected manufacturing additions
                if (model.SelectedAdditionIds != null && model.SelectedAdditionIds.Any())
                {
                    foreach (var additionId in model.SelectedAdditionIds)
                    {
                        var productAddition = new ProductAddition
                        {
                            ProductId = product.Id,
                            ManufacturingAdditionId = additionId,
                            AddedDate = DateTime.Now
                        };
                        await _unitOfWork.ProductAdditions.AddAsync(productAddition);
                    }
                    await _unitOfWork.CompleteAsync();
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Error creating product: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string[] Errors)> UpdateProductAsync(ProductViewModel model)
        {
            try
            {
                var product = await _unitOfWork.Products.GetProductWithDetailsAsync(model.Id);
                if (product == null)
                {
                    return (false, new[] { "Product not found" });
                }

                // Validate product code (excluding current product)
                if (await _unitOfWork.Products.ProductCodeExistsAsync(model.ProductCode, model.Id))
                {
                    return (false, new[] { "A product with this code already exists" });
                }

                // Validate product name (excluding current product)
                if (await _unitOfWork.Products.ProductNameExistsAsync(model.ProductName, model.Id))
                {
                    return (false, new[] { "A product with this name already exists" });
                }

                // Validate supplier exists
                var supplier = await _unitOfWork.Suppliers.GetByIdAsync(model.SupplierId);
                if (supplier == null)
                {
                    return (false, new[] { "Selected supplier not found" });
                }

                // Validate raw material exists
                var rawMaterial = await _unitOfWork.RawMaterials.GetByIdAsync(model.RawMaterialId);
                if (rawMaterial == null)
                {
                    return (false, new[] { "Selected raw material not found" });
                }

                // Validate product code starts with supplier code
                if (!model.ProductCode.StartsWith(supplier.SupplierCode))
                {
                    return (false, new[] { $"Product code must start with supplier code: {supplier.SupplierCode}" });
                }

                product.ProductName = model.ProductName;
                product.ProductCode = model.ProductCode;
                product.IsPrinted = model.IsPrinted;
                product.RawMaterialId = model.RawMaterialId;
                product.SupplierId = model.SupplierId;
                product.Description = model.Description;
                product.LastModified = DateTime.Now;
                product.IsActive = model.IsActive;

                _unitOfWork.Products.Update(product);

                // Update manufacturing additions
                // Delete existing additions
                await _unitOfWork.ProductAdditions.DeleteByProductIdAsync(product.Id);
                await _unitOfWork.CompleteAsync();

                // Add new selections
                if (model.SelectedAdditionIds != null && model.SelectedAdditionIds.Any())
                {
                    foreach (var additionId in model.SelectedAdditionIds)
                    {
                        var productAddition = new ProductAddition
                        {
                            ProductId = product.Id,
                            ManufacturingAdditionId = additionId,
                            AddedDate = DateTime.Now
                        };
                        await _unitOfWork.ProductAdditions.AddAsync(productAddition);
                    }
                }

                await _unitOfWork.CompleteAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Error updating product: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string[] Errors)> DeleteProductAsync(int id)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(id);
                if (product == null)
                {
                    return (false, new[] { "Product not found" });
                }

                // Delete product additions first
                await _unitOfWork.ProductAdditions.DeleteByProductIdAsync(id);

                // Delete product
                _unitOfWork.Products.Delete(product);
                await _unitOfWork.CompleteAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Error deleting product: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string[] Errors)> ToggleProductStatusAsync(int id)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(id);
                if (product == null)
                {
                    return (false, new[] { "Product not found" });
                }

                product.IsActive = !product.IsActive;
                product.LastModified = DateTime.Now;

                _unitOfWork.Products.Update(product);
                await _unitOfWork.CompleteAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Error toggling product status: {ex.Message}" });
            }
        }

        public async Task<string> GetNextProductCodeAsync(int supplierId)
        {
            return await _unitOfWork.Products.GetNextProductCodeAsync(supplierId);
        }

        private async Task PopulateDropdownsAsync(ProductViewModel model)
        {
            // Get active raw materials
            var rawMaterials = await _unitOfWork.RawMaterials.GetActiveRawMaterialsAsync();
            model.RawMaterials = rawMaterials.Select(rm => new SelectListItem
            {
                Value = rm.Id.ToString(),
                Text = rm.RawMaterialName
            }).ToList();

            // Get active suppliers
            var suppliers = await _unitOfWork.Suppliers.GetActiveSuppliersAsync();
            model.Suppliers = suppliers.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = $"{s.SupplierName} ({s.SupplierCode})"
            }).ToList();

            // Get all active manufacturing additions
            var additions = await _unitOfWork.ManufacturingAdditions.GetActiveAdditionsAsync();
            model.AvailableAdditions = additions.Select(a => new ManufacturingAdditionCheckboxViewModel
            {
                Id = a.Id,
                Name = a.AdditionName,
                IsSelected = false
            }).ToList();
        }

        private ProductViewModel MapToViewModel(Product product)
        {
            return new ProductViewModel
            {
                Id = product.Id,
                ProductName = product.ProductName,
                ProductCode = product.ProductCode,
                IsPrinted = product.IsPrinted,
                RawMaterialId = product.RawMaterialId,
                RawMaterialName = product.RawMaterial?.RawMaterialName,
                SupplierId = product.SupplierId,
                SupplierName = product.Supplier?.SupplierName,
                SupplierCode = product.Supplier?.SupplierCode,
                Description = product.Description,
                CreatedDate = product.CreatedDate,
                LastModified = product.LastModified,
                IsActive = product.IsActive,
                SelectedAdditionIds = product.ProductAdditions?
                    .Select(pa => pa.ManufacturingAdditionId)
                    .ToList() ?? new List<int>(),
                SelectedAdditionNames = product.ProductAdditions?
                    .Select(pa => pa.ManufacturingAddition?.AdditionName)
                    .ToList() ?? new List<string>()
            };
        }
    }
}