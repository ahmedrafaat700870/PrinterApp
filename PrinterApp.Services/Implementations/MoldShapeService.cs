using PrinterApp.Data.UnitOfWork;
using PrinterApp.Models.Entities;
using PrinterApp.Models.ViewModels;
using PrinterApp.Services.Interfaces;

namespace PrinterApp.Services.Implementations;

public class MoldShapeService : IMoldShapeService
{
    private readonly IUnitOfWork _unitOfWork;

    public MoldShapeService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<MoldShapeViewModel>> GetAllShapesAsync()
    {
        var shapes = await _unitOfWork.MoldShapes.GetAllAsync();

        var viewModels = new List<MoldShapeViewModel>();
        foreach (var shape in shapes)
        {
            var shapeWithMolds = await _unitOfWork.MoldShapes.GetShapeWithMoldsAsync(shape.Id);
            var viewModel = MapToViewModel(shape);
            viewModel.MoldsCount = shapeWithMolds?.Molds?.Count ?? 0;
            viewModels.Add(viewModel);
        }

        return viewModels.OrderBy(s => s.ShapeName);
    }

    public async Task<IEnumerable<MoldShapeViewModel>> GetActiveShapesAsync()
    {
        var shapes = await _unitOfWork.MoldShapes.GetActiveShapesAsync();
        return shapes.Select(MapToViewModel);
    }

    public async Task<IEnumerable<MoldShapeViewModel>> SearchShapesAsync(string searchTerm)
    {
        var shapes = await _unitOfWork.MoldShapes.GetAllAsync();

        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return shapes.Select(MapToViewModel);
        }

        searchTerm = searchTerm.ToLower().Trim();

        var filteredShapes = shapes.Where(s =>
            s.ShapeName.ToLower().Contains(searchTerm) ||
            (s.Description != null && s.Description.ToLower().Contains(searchTerm))
        );

        return filteredShapes.Select(MapToViewModel);
    }

    public async Task<MoldShapeViewModel> GetShapeByIdAsync(int id)
    {
        var shape = await _unitOfWork.MoldShapes.GetShapeWithMoldsAsync(id);
        if (shape == null) return null;

        var viewModel = MapToViewModel(shape);
        viewModel.MoldsCount = shape.Molds?.Count ?? 0;
        return viewModel;
    }

    public async Task<(bool Success, string[] Errors)> CreateShapeAsync(MoldShapeViewModel model, string imagePath)
    {
        try
        {
            // Validate shape name
            if (await _unitOfWork.MoldShapes.ShapeNameExistsAsync(model.ShapeName))
            {
                return (false, new[] { "A mold shape with this name already exists" });
            }

            var shape = new MoldShape
            {
                ShapeName = model.ShapeName,
                ShapeImagePath = imagePath,
                Description = model.Description,
                CreatedDate = DateTime.Now,
                IsActive = true
            };

            await _unitOfWork.MoldShapes.AddAsync(shape);
            await _unitOfWork.CompleteAsync();

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, new[] { $"Error creating mold shape: {ex.Message}" });
        }
    }

    public async Task<(bool Success, string[] Errors)> UpdateShapeAsync(MoldShapeViewModel model, string imagePath)
    {
        try
        {
            var shape = await _unitOfWork.MoldShapes.GetByIdAsync(model.Id);
            if (shape == null)
            {
                return (false, new[] { "Mold shape not found" });
            }

            // Validate shape name (excluding current shape)
            if (await _unitOfWork.MoldShapes.ShapeNameExistsAsync(model.ShapeName, model.Id))
            {
                return (false, new[] { "A mold shape with this name already exists" });
            }

            shape.ShapeName = model.ShapeName;
            shape.Description = model.Description;
            shape.LastModified = DateTime.Now;
            shape.IsActive = model.IsActive;

            // Update image path if new image was uploaded
            if (!string.IsNullOrEmpty(imagePath))
            {
                shape.ShapeImagePath = imagePath;
            }

            _unitOfWork.MoldShapes.Update(shape);
            await _unitOfWork.CompleteAsync();

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, new[] { $"Error updating mold shape: {ex.Message}" });
        }
    }

    public async Task<(bool Success, string[] Errors)> DeleteShapeAsync(int id)
    {
        try
        {
            var shape = await _unitOfWork.MoldShapes.GetShapeWithMoldsAsync(id);
            if (shape == null)
            {
                return (false, new[] { "Mold shape not found" });
            }

            // Check if shape has associated molds
            if (shape.Molds != null && shape.Molds.Any())
            {
                return (false, new[] { $"Cannot delete this mold shape because it has {shape.Molds.Count} associated mold(s)" });
            }

            _unitOfWork.MoldShapes.Delete(shape);
            await _unitOfWork.CompleteAsync();

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, new[] { $"Error deleting mold shape: {ex.Message}" });
        }
    }

    public async Task<(bool Success, string[] Errors)> ToggleShapeStatusAsync(int id)
    {
        try
        {
            var shape = await _unitOfWork.MoldShapes.GetByIdAsync(id);
            if (shape == null)
            {
                return (false, new[] { "Mold shape not found" });
            }

            shape.IsActive = !shape.IsActive;
            shape.LastModified = DateTime.Now;

            _unitOfWork.MoldShapes.Update(shape);
            await _unitOfWork.CompleteAsync();

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, new[] { $"Error toggling mold shape status: {ex.Message}" });
        }
    }

    private MoldShapeViewModel MapToViewModel(MoldShape shape)
    {
        return new MoldShapeViewModel
        {
            Id = shape.Id,
            ShapeName = shape.ShapeName,
            ShapeImagePath = shape.ShapeImagePath,
            Description = shape.Description,
            CreatedDate = shape.CreatedDate,
            LastModified = shape.LastModified,
            IsActive = shape.IsActive
        };
    }
}