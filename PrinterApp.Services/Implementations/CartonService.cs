using PrinterApp.Data.UnitOfWork;
using PrinterApp.Models.Entities;
using PrinterApp.Models.ViewModels;
using PrinterApp.Services.Interfaces;

namespace PrinterApp.Services.Implementations
{
    public class CartonService : ICartonService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartonService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CartonViewModel>> GetAllCartonsAsync()
        {
            var cartons = await _unitOfWork.Cartons.GetAllAsync();
            return cartons.Select(MapToViewModel).OrderBy(c => c.CartonName);
        }

        public async Task<IEnumerable<CartonViewModel>> GetActiveCartonsAsync()
        {
            var cartons = await _unitOfWork.Cartons.GetActiveCartonsAsync();
            return cartons.Select(MapToViewModel);
        }

        public async Task<IEnumerable<CartonViewModel>> SearchCartonsAsync(string searchTerm)
        {
            var cartons = await _unitOfWork.Cartons.GetAllAsync();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return cartons.Select(MapToViewModel).OrderBy(c => c.CartonName);
            }

            searchTerm = searchTerm.ToLower().Trim();

            var filteredCartons = cartons.Where(c =>
                c.CartonName.ToLower().Contains(searchTerm) ||
                c.CartonFactor.ToString().Contains(searchTerm) ||
                (!string.IsNullOrEmpty(c.Description) && c.Description.ToLower().Contains(searchTerm))
            );

            return filteredCartons.Select(MapToViewModel).OrderBy(c => c.CartonName);
        }

        public async Task<CartonViewModel> GetCartonByIdAsync(int id)
        {
            var carton = await _unitOfWork.Cartons.GetByIdAsync(id);
            return carton != null ? MapToViewModel(carton) : null;
        }

        public async Task<(bool Success, string[] Errors)> CreateCartonAsync(CartonViewModel model)
        {
            try
            {
                // Check if carton name already exists
                if (await _unitOfWork.Cartons.CartonNameExistsAsync(model.CartonName))
                {
                    return (false, new[] { "A carton with this name already exists" });
                }

                var carton = new Carton
                {
                    CartonName = model.CartonName,
                    CartonFactor = model.CartonFactor,
                    Description = model.Description,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };

                await _unitOfWork.Cartons.AddAsync(carton);
                await _unitOfWork.CompleteAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Error creating carton: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string[] Errors)> UpdateCartonAsync(CartonViewModel model)
        {
            try
            {
                var carton = await _unitOfWork.Cartons.GetByIdAsync(model.Id);
                if (carton == null)
                {
                    return (false, new[] { "Carton not found" });
                }

                // Check if new name conflicts with existing carton
                if (await _unitOfWork.Cartons.CartonNameExistsAsync(model.CartonName, model.Id))
                {
                    return (false, new[] { "A carton with this name already exists" });
                }

                carton.CartonName = model.CartonName;
                carton.CartonFactor = model.CartonFactor;
                carton.Description = model.Description;
                carton.LastModified = DateTime.Now;
                carton.IsActive = model.IsActive;

                _unitOfWork.Cartons.Update(carton);
                await _unitOfWork.CompleteAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Error updating carton: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string[] Errors)> DeleteCartonAsync(int id)
        {
            try
            {
                var carton = await _unitOfWork.Cartons.GetByIdAsync(id);
                if (carton == null)
                {
                    return (false, new[] { "Carton not found" });
                }

                _unitOfWork.Cartons.Delete(carton);
                await _unitOfWork.CompleteAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Error deleting carton: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string[] Errors)> ToggleCartonStatusAsync(int id)
        {
            try
            {
                var carton = await _unitOfWork.Cartons.GetByIdAsync(id);
                if (carton == null)
                {
                    return (false, new[] { "Carton not found" });
                }

                carton.IsActive = !carton.IsActive;
                carton.LastModified = DateTime.Now;

                _unitOfWork.Cartons.Update(carton);
                await _unitOfWork.CompleteAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Error toggling carton status: {ex.Message}" });
            }
        }

        private CartonViewModel MapToViewModel(Carton carton)
        {
            return new CartonViewModel
            {
                Id = carton.Id,
                CartonName = carton.CartonName,
                CartonFactor = carton.CartonFactor,
                Description = carton.Description,
                CreatedDate = carton.CreatedDate,
                LastModified = carton.LastModified,
                IsActive = carton.IsActive
            };
        }
    }
}