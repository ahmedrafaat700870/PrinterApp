using PrinterApp.Data.UnitOfWork;
using PrinterApp.Models.Entities;
using PrinterApp.Models.ViewModels;
using PrinterApp.Services.Interfaces;

namespace PrinterApp.Services.Implementations
{
    public class RawMaterialService : IRawMaterialService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RawMaterialService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<RawMaterialViewModel>> GetAllRawMaterialsAsync()
        {
            var rawMaterials = await _unitOfWork.RawMaterials.GetAllAsync();
            return rawMaterials.Select(MapToViewModel).OrderBy(r => r.RawMaterialName);
        }

        public async Task<IEnumerable<RawMaterialViewModel>> GetActiveRawMaterialsAsync()
        {
            var rawMaterials = await _unitOfWork.RawMaterials.GetActiveRawMaterialsAsync();
            return rawMaterials.Select(MapToViewModel);
        }

        public async Task<IEnumerable<RawMaterialViewModel>> SearchRawMaterialsAsync(string searchTerm)
        {
            var rawMaterials = await _unitOfWork.RawMaterials.GetAllAsync();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return rawMaterials.Select(MapToViewModel).OrderBy(r => r.RawMaterialName);
            }

            searchTerm = searchTerm.ToLower().Trim();

            var filteredRawMaterials = rawMaterials.Where(r =>
                r.RawMaterialName.ToLower().Contains(searchTerm) ||
                r.Width.ToString().Contains(searchTerm) ||
                r.Height.ToString().Contains(searchTerm) ||
                r.TotalPrice.ToString().Contains(searchTerm)
            );

            return filteredRawMaterials.Select(MapToViewModel).OrderBy(r => r.RawMaterialName);
        }

        public async Task<RawMaterialViewModel> GetRawMaterialByIdAsync(int id)
        {
            var rawMaterial = await _unitOfWork.RawMaterials.GetByIdAsync(id);
            return rawMaterial != null ? MapToViewModel(rawMaterial) : null;
        }

        public async Task<(bool Success, string[] Errors)> CreateRawMaterialAsync(RawMaterialViewModel model)
        {
            try
            {
                // Check if raw material name already exists
                if (await _unitOfWork.RawMaterials.RawMaterialNameExistsAsync(model.RawMaterialName))
                {
                    return (false, new[] { "A raw material with this name already exists" });
                }

                // Calculate area in square meters
                // Width in cm / 100 = width in meters
                // Height is already in meters
                var widthInMeters = model.Width / 100;
                var heightInMeters = model.Height;
                var areaSquareMeters = widthInMeters * heightInMeters;

                // Calculate price per square meter
                var pricePerSquareMeter = areaSquareMeters > 0 ? model.TotalPrice / areaSquareMeters : 0;

                // Calculate price per linear meter (based on height)
                var pricePerLinearMeter = heightInMeters > 0 ? model.TotalPrice / heightInMeters : 0;

                var rawMaterial = new RawMaterial
                {
                    RawMaterialName = model.RawMaterialName,
                    Width = model.Width,
                    Height = model.Height,
                    TotalPrice = model.TotalPrice,
                    AreaSquareMeters = areaSquareMeters,
                    PricePerSquareMeter = pricePerSquareMeter,
                    PricePerLinearMeter = pricePerLinearMeter,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };

                await _unitOfWork.RawMaterials.AddAsync(rawMaterial);
                await _unitOfWork.CompleteAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Error creating raw material: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string[] Errors)> UpdateRawMaterialAsync(RawMaterialViewModel model)
        {
            try
            {
                var rawMaterial = await _unitOfWork.RawMaterials.GetByIdAsync(model.Id);
                if (rawMaterial == null)
                {
                    return (false, new[] { "Raw material not found" });
                }

                // Check if new name conflicts with existing raw material
                if (await _unitOfWork.RawMaterials.RawMaterialNameExistsAsync(model.RawMaterialName, model.Id))
                {
                    return (false, new[] { "A raw material with this name already exists" });
                }

                // Recalculate area and prices
                var widthInMeters = model.Width / 100;
                var heightInMeters = model.Height;
                var areaSquareMeters = widthInMeters * heightInMeters;
                var pricePerSquareMeter = areaSquareMeters > 0 ? model.TotalPrice / areaSquareMeters : 0;
                var pricePerLinearMeter = heightInMeters > 0 ? model.TotalPrice / heightInMeters : 0;

                rawMaterial.RawMaterialName = model.RawMaterialName;
                rawMaterial.Width = model.Width;
                rawMaterial.Height = model.Height;
                rawMaterial.TotalPrice = model.TotalPrice;
                rawMaterial.AreaSquareMeters = areaSquareMeters;
                rawMaterial.PricePerSquareMeter = pricePerSquareMeter;
                rawMaterial.PricePerLinearMeter = pricePerLinearMeter;
                rawMaterial.LastModified = DateTime.Now;
                rawMaterial.IsActive = model.IsActive;

                _unitOfWork.RawMaterials.Update(rawMaterial);
                await _unitOfWork.CompleteAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Error updating raw material: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string[] Errors)> DeleteRawMaterialAsync(int id)
        {
            try
            {
                var rawMaterial = await _unitOfWork.RawMaterials.GetByIdAsync(id);
                if (rawMaterial == null)
                {
                    return (false, new[] { "Raw material not found" });
                }

                _unitOfWork.RawMaterials.Delete(rawMaterial);
                await _unitOfWork.CompleteAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Error deleting raw material: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string[] Errors)> ToggleRawMaterialStatusAsync(int id)
        {
            try
            {
                var rawMaterial = await _unitOfWork.RawMaterials.GetByIdAsync(id);
                if (rawMaterial == null)
                {
                    return (false, new[] { "Raw material not found" });
                }

                rawMaterial.IsActive = !rawMaterial.IsActive;
                rawMaterial.LastModified = DateTime.Now;

                _unitOfWork.RawMaterials.Update(rawMaterial);
                await _unitOfWork.CompleteAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Error toggling raw material status: {ex.Message}" });
            }
        }

        private RawMaterialViewModel MapToViewModel(RawMaterial rawMaterial)
        {
            return new RawMaterialViewModel
            {
                Id = rawMaterial.Id,
                RawMaterialName = rawMaterial.RawMaterialName,
                Width = rawMaterial.Width,
                Height = rawMaterial.Height,
                TotalPrice = rawMaterial.TotalPrice,
                AreaSquareMeters = rawMaterial.AreaSquareMeters,
                PricePerSquareMeter = rawMaterial.PricePerSquareMeter,
                PricePerLinearMeter = rawMaterial.PricePerLinearMeter,
                CreatedDate = rawMaterial.CreatedDate,
                LastModified = rawMaterial.LastModified,
                IsActive = rawMaterial.IsActive
            };
        }
    }
}