using PrinterApp.Data.UnitOfWork;
using PrinterApp.Models.Entities;
using PrinterApp.Models.ViewModels;
using PrinterApp.Services.Interfaces;

namespace PrinterApp.Services.Implementations
{
    public class KnifeService : IKnifeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public KnifeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<KnifeViewModel>> GetAllKnivesAsync()
        {
            var knives = await _unitOfWork.Knives.GetAllAsync();
            return knives.Select(MapToViewModel).OrderBy(k => k.KnifeName);
        }

        public async Task<IEnumerable<KnifeViewModel>> GetActiveKnivesAsync()
        {
            var knives = await _unitOfWork.Knives.GetActiveKnivesAsync();
            return knives.Select(MapToViewModel);
        }

        public async Task<IEnumerable<KnifeViewModel>> SearchKnivesAsync(string searchTerm)
        {
            var knives = await _unitOfWork.Knives.GetAllAsync();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return knives.Select(MapToViewModel).OrderBy(k => k.KnifeName);
            }

            searchTerm = searchTerm.ToLower().Trim();

            var filteredKnives = knives.Where(k =>
                k.KnifeName.ToLower().Contains(searchTerm) ||
                k.KnifeFactor.ToString().Contains(searchTerm) ||
                (!string.IsNullOrEmpty(k.Description) && k.Description.ToLower().Contains(searchTerm))
            );

            return filteredKnives.Select(MapToViewModel).OrderBy(k => k.KnifeName);
        }

        public async Task<KnifeViewModel> GetKnifeByIdAsync(int id)
        {
            var knife = await _unitOfWork.Knives.GetByIdAsync(id);
            return knife != null ? MapToViewModel(knife) : null;
        }

        public async Task<(bool Success, string[] Errors)> CreateKnifeAsync(KnifeViewModel model)
        {
            try
            {
                // Check if knife name already exists
                if (await _unitOfWork.Knives.KnifeNameExistsAsync(model.KnifeName))
                {
                    return (false, new[] { "A knife with this name already exists" });
                }

                var knife = new Knife
                {
                    KnifeName = model.KnifeName,
                    KnifeFactor = model.KnifeFactor,
                    Description = model.Description,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };

                await _unitOfWork.Knives.AddAsync(knife);
                await _unitOfWork.CompleteAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Error creating knife: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string[] Errors)> UpdateKnifeAsync(KnifeViewModel model)
        {
            try
            {
                var knife = await _unitOfWork.Knives.GetByIdAsync(model.Id);
                if (knife == null)
                {
                    return (false, new[] { "Knife not found" });
                }

                // Check if new name conflicts with existing knife
                if (await _unitOfWork.Knives.KnifeNameExistsAsync(model.KnifeName, model.Id))
                {
                    return (false, new[] { "A knife with this name already exists" });
                }

                knife.KnifeName = model.KnifeName;
                knife.KnifeFactor = model.KnifeFactor;
                knife.Description = model.Description;
                knife.LastModified = DateTime.Now;
                knife.IsActive = model.IsActive;

                _unitOfWork.Knives.Update(knife);
                await _unitOfWork.CompleteAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Error updating knife: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string[] Errors)> DeleteKnifeAsync(int id)
        {
            try
            {
                var knife = await _unitOfWork.Knives.GetByIdAsync(id);
                if (knife == null)
                {
                    return (false, new[] { "Knife not found" });
                }

                _unitOfWork.Knives.Delete(knife);
                await _unitOfWork.CompleteAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Error deleting knife: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string[] Errors)> ToggleKnifeStatusAsync(int id)
        {
            try
            {
                var knife = await _unitOfWork.Knives.GetByIdAsync(id);
                if (knife == null)
                {
                    return (false, new[] { "Knife not found" });
                }

                knife.IsActive = !knife.IsActive;
                knife.LastModified = DateTime.Now;

                _unitOfWork.Knives.Update(knife);
                await _unitOfWork.CompleteAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Error toggling knife status: {ex.Message}" });
            }
        }

        private KnifeViewModel MapToViewModel(Knife knife)
        {
            return new KnifeViewModel
            {
                Id = knife.Id,
                KnifeName = knife.KnifeName,
                KnifeFactor = knife.KnifeFactor,
                Description = knife.Description,
                CreatedDate = knife.CreatedDate,
                LastModified = knife.LastModified,
                IsActive = knife.IsActive
            };
        }
    }
}