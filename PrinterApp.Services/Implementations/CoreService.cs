using Microsoft.EntityFrameworkCore.Storage.Json;
using PrinterApp.Data.UnitOfWork;
using PrinterApp.Models.Entities;
using PrinterApp.Models.ViewModels;
using PrinterApp.Services.Interfaces;
namespace PrinterApp.Services.Implementations;

public class CoreService : ICoreService
{
    private readonly IUnitOfWork _unitOfWork;
    public CoreService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<(bool Success, string[] Errors)> CreateCoreAsync(CoreViewModel model)
    {
        // check if core name exsists
        if (await _unitOfWork.Cores.CoreNameExistsAsync(model.CoreName))
            return (false, new string[] { "Core name already exists." });
        var core = new Core()
        {
            CoreName = model.CoreName,
            CoreCoefficient = model.CoreCoefficient,
            CreatedDate = DateTime.UtcNow,
            HeightCor = model.HeightCor,
            WidthCor = model.WidthCor,
            IsActive = model.IsActive,
            LastModified = model.LastModified,
        };
        try
        {
            await _unitOfWork.Cores.AddAsync(core);
            await _unitOfWork.CompleteAsync();
            return (true, Array.Empty<string>());
        }
        catch (Exception ex)
        {
            return (false, new string[] { "Error Creating Core: " + ex.Message });
        }
    }

    public async Task<(bool Success, string[] Errors)> DeleteCoreAsync(int id)
    {
        var deletedCore = await _unitOfWork.Cores.GetByIdAsync(id);
        if (deletedCore is null)
        {
            return (false, new string[] { "Core not found ." });
        }
        try
        {
            _unitOfWork.Cores.Delete(deletedCore);
            await _unitOfWork.CompleteAsync();
            return (true, Array.Empty<string>());

        }
        catch (Exception ex)
        {
            return (false, new string[] { "Error Deleting Core: " + ex.Message });
        }
    }

    public async Task<IEnumerable<CoreViewModel>> GetActiveCoresAsync()
    {
        return (await _unitOfWork.Cores.GetActiveCoresAsync()).Select(MapToViewModel).OrderBy(x => x.CoreName);
    }

    public async Task<IEnumerable<CoreViewModel>> GetAllCoresAsync()
    {
        return (await _unitOfWork.Cores.GetAllAsync()).Select(MapToViewModel).OrderBy(x => x.CoreName);
    }

    public async Task<CoreViewModel> GetCoreByIdAsync(int id)
    {
        var core = await this._unitOfWork.Cores.GetByIdAsync(id);
        return core != null ? MapToViewModel(core) : null;
    }

    public async Task<IEnumerable<CoreViewModel>> SearchCoresAsync(string searchTerm)
    {
        var cores = await _unitOfWork.Cores.GetAllAsync();

        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return cores.Select(MapToViewModel).OrderBy(c => c.CoreName);
        }

        searchTerm = searchTerm.ToLower().Trim();

        var filteredCores = cores.Where(c =>
            c.CoreName.ToLower().Contains(searchTerm) ||
            c.CoreCoefficient.ToString().Contains(searchTerm) ||
            c.WidthCor.ToString().Contains(searchTerm) ||
            c.HeightCor.ToString().Contains(searchTerm)
        );

        return filteredCores.Select(MapToViewModel).OrderBy(c => c.CoreName);
    }

    public async Task<(bool Success, string[] Errors)> ToggleCoreStatusAsync(int id)
    {
        var core = await this._unitOfWork.Cores.GetByIdAsync(id);
        if (core is null)
            return (false, new string[] { "Core not found." });

        core.IsActive = !core.IsActive;
        try
        {
            _unitOfWork.Cores.Update(core);
            await _unitOfWork.CompleteAsync();
            return (true, Array.Empty<string>());
        }
        catch (Exception ex)
        {
            return (false, new string[] { "Error Toggling Core Status: " + ex.Message });
        }
    }

    public async Task<(bool Success, string[] Errors)> UpdateCoreAsync(CoreViewModel model)
    {
        var core = await _unitOfWork.Cores.GetByIdAsync(model.Id);
        if (core is null)
            return (false, new string[] { "Core not found." });

        core.CoreName = model.CoreName;
        core.CoreCoefficient = model.CoreCoefficient;
        core.WidthCor = model.WidthCor;
        core.HeightCor = model.HeightCor;
        core.LastModified = DateTime.Now;
        core.IsActive = model.IsActive;
        try
        {
            _unitOfWork.Cores.Update(core);
            await _unitOfWork.CompleteAsync();
            return (true, Array.Empty<string>());
        }
        catch (Exception ex)
        {
            return (false, new string[] { "Error Updating Core: " + ex.Message });
        }
    }


    private CoreViewModel MapToViewModel(Core core)
    {
        return new CoreViewModel
        {
            Id = core.Id,
            CoreName = core.CoreName,
            CoreCoefficient = core.CoreCoefficient,
            WidthCor = core.WidthCor,
            HeightCor = core.HeightCor,
            CreatedDate = core.CreatedDate,
            LastModified = core.LastModified,
            IsActive = core.IsActive
        };
    }
}
