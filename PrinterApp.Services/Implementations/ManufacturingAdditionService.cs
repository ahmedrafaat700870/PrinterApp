using PrinterApp.Data.UnitOfWork;
using PrinterApp.Models.Entities;
using PrinterApp.Models.ViewModels;
using PrinterApp.Services.Interfaces;

namespace PrinterApp.Services.Implementations
{
    public class ManufacturingAdditionService : IManufacturingAdditionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ManufacturingAdditionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ManufacturingAdditionViewModel>> GetAllAdditionsAsync()
        {
            var additions = await _unitOfWork.ManufacturingAdditions.GetAllAsync();
            return additions.Select(MapToViewModel).OrderBy(a => a.AdditionName);
        }

        public async Task<IEnumerable<ManufacturingAdditionViewModel>> GetActiveAdditionsAsync()
        {
            var additions = await _unitOfWork.ManufacturingAdditions.GetActiveAdditionsAsync();
            return additions.Select(MapToViewModel);
        }

        public async Task<IEnumerable<ManufacturingAdditionViewModel>> SearchAdditionsAsync(string searchTerm)
        {
            var additions = await _unitOfWork.ManufacturingAdditions.GetAllAsync();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return additions.Select(MapToViewModel).OrderBy(a => a.AdditionName);
            }

            searchTerm = searchTerm.ToLower().Trim();

            var filteredAdditions = additions.Where(a =>
                a.AdditionName.ToLower().Contains(searchTerm)
            );

            return filteredAdditions.Select(MapToViewModel).OrderBy(a => a.AdditionName);
        }

        public async Task<ManufacturingAdditionViewModel> GetAdditionByIdAsync(int id)
        {
            var addition = await _unitOfWork.ManufacturingAdditions.GetByIdAsync(id);
            return addition != null ? MapToViewModel(addition) : null;
        }

        public async Task<(bool Success, string[] Errors)> CreateAdditionAsync(ManufacturingAdditionViewModel model)
        {
            try
            {
                // Check if addition name already exists
                if (await _unitOfWork.ManufacturingAdditions.AdditionNameExistsAsync(model.AdditionName))
                {
                    return (false, new[] { "An addition with this name already exists" });
                }

                var addition = new ManufacturingAddition
                {
                    AdditionName = model.AdditionName,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };

                await _unitOfWork.ManufacturingAdditions.AddAsync(addition);
                await _unitOfWork.CompleteAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Error creating addition: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string[] Errors)> UpdateAdditionAsync(ManufacturingAdditionViewModel model)
        {
            try
            {
                var addition = await _unitOfWork.ManufacturingAdditions.GetByIdAsync(model.Id);
                if (addition == null)
                {
                    return (false, new[] { "Addition not found" });
                }

                // Check if new name conflicts with existing addition
                if (await _unitOfWork.ManufacturingAdditions.AdditionNameExistsAsync(model.AdditionName, model.Id))
                {
                    return (false, new[] { "An addition with this name already exists" });
                }

                addition.AdditionName = model.AdditionName;
                addition.LastModified = DateTime.Now;
                addition.IsActive = model.IsActive;

                _unitOfWork.ManufacturingAdditions.Update(addition);
                await _unitOfWork.CompleteAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Error updating addition: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string[] Errors)> DeleteAdditionAsync(int id)
        {
            try
            {
                var addition = await _unitOfWork.ManufacturingAdditions.GetByIdAsync(id);
                if (addition == null)
                {
                    return (false, new[] { "Addition not found" });
                }

                _unitOfWork.ManufacturingAdditions.Delete(addition);
                await _unitOfWork.CompleteAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Error deleting addition: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string[] Errors)> ToggleAdditionStatusAsync(int id)
        {
            try
            {
                var addition = await _unitOfWork.ManufacturingAdditions.GetByIdAsync(id);
                if (addition == null)
                {
                    return (false, new[] { "Addition not found" });
                }

                addition.IsActive = !addition.IsActive;
                addition.LastModified = DateTime.Now;

                _unitOfWork.ManufacturingAdditions.Update(addition);
                await _unitOfWork.CompleteAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Error toggling addition status: {ex.Message}" });
            }
        }

        private ManufacturingAdditionViewModel MapToViewModel(ManufacturingAddition addition)
        {
            return new ManufacturingAdditionViewModel
            {
                Id = addition.Id,
                AdditionName = addition.AdditionName,
                CreatedDate = addition.CreatedDate,
                LastModified = addition.LastModified,
                IsActive = addition.IsActive
            };
        }
    }
}