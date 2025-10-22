using PrinterApp.Data.UnitOfWork;
using PrinterApp.Models.Entities;
using PrinterApp.Models.ViewModels;
using PrinterApp.Services.Interfaces;

namespace PrinterApp.Services.Implementations
{
    public class RollDirectionService : IRollDirectionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RollDirectionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<RollDirectionViewModel>> GetAllDirectionsAsync()
        {
            var directions = await _unitOfWork.RollDirections.GetAllAsync();
            return directions.Select(MapToViewModel).OrderBy(d => d.DirectionNumber);
        }

        public async Task<IEnumerable<RollDirectionViewModel>> GetActiveDirectionsAsync()
        {
            var directions = await _unitOfWork.RollDirections.GetActiveDirectionsAsync();
            return directions.Select(MapToViewModel);
        }

        public async Task<IEnumerable<RollDirectionViewModel>> SearchDirectionsAsync(string searchTerm)
        {
            var directions = await _unitOfWork.RollDirections.GetAllAsync();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return directions.Select(MapToViewModel).OrderBy(d => d.DirectionNumber);
            }

            searchTerm = searchTerm.ToLower().Trim();

            var filteredDirections = directions.Where(d =>
                d.DirectionNumber.ToString().Contains(searchTerm) ||
                (!string.IsNullOrEmpty(d.Description) && d.Description.ToLower().Contains(searchTerm))
            );

            return filteredDirections.Select(MapToViewModel).OrderBy(d => d.DirectionNumber);
        }

        public async Task<RollDirectionViewModel> GetDirectionByIdAsync(int id)
        {
            var direction = await _unitOfWork.RollDirections.GetByIdAsync(id);
            return direction != null ? MapToViewModel(direction) : null;
        }

        public async Task<(bool Success, string[] Errors)> CreateDirectionAsync(RollDirectionViewModel model, string imagePath)
        {
            try
            {
                if (await _unitOfWork.RollDirections.DirectionNumberExistsAsync(model.DirectionNumber))
                {
                    return (false, new[] { "A direction with this number already exists" });
                }

                var direction = new RollDirection
                {
                    DirectionNumber = model.DirectionNumber,
                    DirectionImage = imagePath,
                    Description = model.Description,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };

                await _unitOfWork.RollDirections.AddAsync(direction);
                await _unitOfWork.CompleteAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Error creating direction: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string[] Errors)> UpdateDirectionAsync(RollDirectionViewModel model, string imagePath)
        {
            try
            {
                var direction = await _unitOfWork.RollDirections.GetByIdAsync(model.Id);
                if (direction == null)
                {
                    return (false, new[] { "Direction not found" });
                }

                if (await _unitOfWork.RollDirections.DirectionNumberExistsAsync(model.DirectionNumber, model.Id))
                {
                    return (false, new[] { "A direction with this number already exists" });
                }

                if (!string.IsNullOrEmpty(imagePath))
                {
                    direction.DirectionImage = imagePath;
                }

                direction.DirectionNumber = model.DirectionNumber;
                direction.Description = model.Description;
                direction.LastModified = DateTime.Now;
                direction.IsActive = model.IsActive;

                _unitOfWork.RollDirections.Update(direction);
                await _unitOfWork.CompleteAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Error updating direction: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string[] Errors)> DeleteDirectionAsync(int id, string deleteImagePath)
        {
            try
            {
                var direction = await _unitOfWork.RollDirections.GetByIdAsync(id);
                if (direction == null)
                {
                    return (false, new[] { "Direction not found" });
                }

                _unitOfWork.RollDirections.Delete(direction);
                await _unitOfWork.CompleteAsync();

                return (true, new[] { deleteImagePath }); // Return image path to delete in controller
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Error deleting direction: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string[] Errors)> ToggleDirectionStatusAsync(int id)
        {
            try
            {
                var direction = await _unitOfWork.RollDirections.GetByIdAsync(id);
                if (direction == null)
                {
                    return (false, new[] { "Direction not found" });
                }

                direction.IsActive = !direction.IsActive;
                direction.LastModified = DateTime.Now;

                _unitOfWork.RollDirections.Update(direction);
                await _unitOfWork.CompleteAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Error toggling direction status: {ex.Message}" });
            }
        }

        private RollDirectionViewModel MapToViewModel(RollDirection direction)
        {
            return new RollDirectionViewModel
            {
                Id = direction.Id,
                DirectionNumber = direction.DirectionNumber,
                DirectionImage = direction.DirectionImage,
                Description = direction.Description,
                CreatedDate = direction.CreatedDate,
                LastModified = direction.LastModified,
                IsActive = direction.IsActive
            };
        }
    }
}