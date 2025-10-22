using Microsoft.AspNetCore.Mvc.Rendering;
using PrinterApp.Data.UnitOfWork;
using PrinterApp.Models.Entities;
using PrinterApp.Models.ViewModels;
using PrinterApp.Services.Interfaces;

namespace PrinterApp.Services.Implementations;

public class MoldService : IMoldService
{
    private readonly IUnitOfWork _unitOfWork;

    public MoldService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<MoldViewModel>> GetAllMoldsAsync()
    {
        var molds = await _unitOfWork.Molds.GetMoldsWithDetailsAsync();
        return molds.Select(MapToViewModel);
    }

    public async Task<IEnumerable<MoldViewModel>> GetActiveMoldsAsync()
    {
        var molds = await _unitOfWork.Molds.GetActiveMoldsAsync();
        return molds.Select(MapToViewModel);
    }

    public async Task<IEnumerable<MoldViewModel>> SearchMoldsAsync(string searchTerm)
    {
        var molds = await _unitOfWork.Molds.GetMoldsWithDetailsAsync();

        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return molds.Select(MapToViewModel);
        }

        searchTerm = searchTerm.ToLower().Trim();

        var filteredMolds = molds.Where(m =>
            m.MoldNumber.ToLower().Contains(searchTerm) ||
            m.Machine.MachineName.ToLower().Contains(searchTerm) ||
            m.MoldShape.ShapeName.ToLower().Contains(searchTerm)
        );

        return filteredMolds.Select(MapToViewModel);
    }

    public async Task<MoldViewModel> GetMoldByIdAsync(int id)
    {
        var mold = await _unitOfWork.Molds.GetMoldWithDetailsAsync(id);
        return mold != null ? MapToViewModel(mold) : null;
    }

    public async Task<MoldViewModel> GetMoldForCreateAsync()
    {
        var model = new MoldViewModel();
        await PopulateDropdownsAsync(model);
        return model;
    }

    public async Task<MoldViewModel> GetMoldForEditAsync(int id)
    {
        var mold = await _unitOfWork.Molds.GetMoldWithDetailsAsync(id);
        if (mold == null) return null;

        var model = MapToViewModel(mold);
        await PopulateDropdownsAsync(model);
        return model;
    }

    public async Task<(bool Success, string[] Errors)> CreateMoldAsync(MoldViewModel model)
    {
        try
        {
            // Validate mold number
            if (await _unitOfWork.Molds.MoldNumberExistsAsync(model.MoldNumber))
            {
                return (false, new[] { "A mold with this number already exists" });
            }

            // Validate machine exists
            var machine = await _unitOfWork.Machines.GetByIdAsync(model.MachineId);
            if (machine == null)
            {
                return (false, new[] { "Selected machine not found" });
            }

            // Validate mold shape exists
            var moldShape = await _unitOfWork.MoldShapes.GetByIdAsync(model.MoldShapeId);
            if (moldShape == null)
            {
                return (false, new[] { "Selected mold shape not found" });
            }

            // Calculate total eyes
            var totalEyes = CalculateTotalEyes(model.Width, model.Height);

            var mold = new Mold
            {
                MoldNumber = model.MoldNumber,
                MachineId = model.MachineId,
                MoldShapeId = model.MoldShapeId,
                Width = model.Width,
                Height = model.Height,
                TotalEyes = totalEyes,
                PrintedRawMaterialSize = model.PrintedRawMaterialSize,
                PlainRawMaterialSize = model.PlainRawMaterialSize,
                Description = model.Description,
                CreatedDate = DateTime.Now,
                IsActive = true
            };

            await _unitOfWork.Molds.AddAsync(mold);
            await _unitOfWork.CompleteAsync();

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, new[] { $"Error creating mold: {ex.Message}" });
        }
    }

    public async Task<(bool Success, string[] Errors)> UpdateMoldAsync(MoldViewModel model)
    {
        try
        {
            var mold = await _unitOfWork.Molds.GetMoldWithDetailsAsync(model.Id);
            if (mold == null)
            {
                return (false, new[] { "Mold not found" });
            }

            // Validate mold number (excluding current mold)
            if (await _unitOfWork.Molds.MoldNumberExistsAsync(model.MoldNumber, model.Id))
            {
                return (false, new[] { "A mold with this number already exists" });
            }

            // Validate machine exists
            var machine = await _unitOfWork.Machines.GetByIdAsync(model.MachineId);
            if (machine == null)
            {
                return (false, new[] { "Selected machine not found" });
            }

            // Validate mold shape exists
            var moldShape = await _unitOfWork.MoldShapes.GetByIdAsync(model.MoldShapeId);
            if (moldShape == null)
            {
                return (false, new[] { "Selected mold shape not found" });
            }

            // Calculate total eyes
            var totalEyes = CalculateTotalEyes(model.Width, model.Height);

            mold.MoldNumber = model.MoldNumber;
            mold.MachineId = model.MachineId;
            mold.MoldShapeId = model.MoldShapeId;
            mold.Width = model.Width;
            mold.Height = model.Height;
            mold.TotalEyes = totalEyes;
            mold.PrintedRawMaterialSize = model.PrintedRawMaterialSize;
            mold.PlainRawMaterialSize = model.PlainRawMaterialSize;
            mold.Description = model.Description;
            mold.LastModified = DateTime.Now;
            mold.IsActive = model.IsActive;

            _unitOfWork.Molds.Update(mold);
            await _unitOfWork.CompleteAsync();

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, new[] { $"Error updating mold: {ex.Message}" });
        }
    }

    public async Task<(bool Success, string[] Errors)> DeleteMoldAsync(int id)
    {
        try
        {
            var mold = await _unitOfWork.Molds.GetByIdAsync(id);
            if (mold == null)
            {
                return (false, new[] { "Mold not found" });
            }

            _unitOfWork.Molds.Delete(mold);
            await _unitOfWork.CompleteAsync();

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, new[] { $"Error deleting mold: {ex.Message}" });
        }
    }

    public async Task<(bool Success, string[] Errors)> ToggleMoldStatusAsync(int id)
    {
        try
        {
            var mold = await _unitOfWork.Molds.GetByIdAsync(id);
            if (mold == null)
            {
                return (false, new[] { "Mold not found" });
            }

            mold.IsActive = !mold.IsActive;
            mold.LastModified = DateTime.Now;

            _unitOfWork.Molds.Update(mold);
            await _unitOfWork.CompleteAsync();

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, new[] { $"Error toggling mold status: {ex.Message}" });
        }
    }

    public int CalculateTotalEyes(int width, int height)
    {
        return width * height;
    }

    private async Task PopulateDropdownsAsync(MoldViewModel model)
    {
        // Get active machines
        var machines = await _unitOfWork.Machines.GetActiveMachinesAsync();
        model.Machines = machines.Select(m => new SelectListItem
        {
            Value = m.Id.ToString(),
            Text = $"{m.MachineName} ({m.ModelNumber})"
        }).ToList();

        // Get active mold shapes
        var shapes = await _unitOfWork.MoldShapes.GetActiveShapesAsync();
        model.MoldShapes = shapes.Select(s => new SelectListItem
        {
            Value = s.Id.ToString(),
            Text = s.ShapeName
        }).ToList();
    }

    private MoldViewModel MapToViewModel(Mold mold)
    {
        return new MoldViewModel
        {
            Id = mold.Id,
            MoldNumber = mold.MoldNumber,
            MachineId = mold.MachineId,
            MachineName = mold.Machine?.MachineName,
            MachineCode = mold.Machine?.ModelNumber,
            MoldShapeId = mold.MoldShapeId,
            MoldShapeName = mold.MoldShape?.ShapeName,
            ShapeImagePath = mold.MoldShape?.ShapeImagePath,
            Width = mold.Width,
            Height = mold.Height,
            TotalEyes = mold.TotalEyes,
            PrintedRawMaterialSize = mold.PrintedRawMaterialSize,
            PlainRawMaterialSize = mold.PlainRawMaterialSize,
            Description = mold.Description,
            CreatedDate = mold.CreatedDate,
            LastModified = mold.LastModified,
            IsActive = mold.IsActive
        };
    }
}