using PrinterApp.Data.UnitOfWork;
using PrinterApp.Models.Entities;
using PrinterApp.Models.ViewModels;
using PrinterApp.Services.Interfaces;

namespace PrinterApp.Services.Implementations
{
    public class MachineService : IMachineService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MachineService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<MachineViewModel>> GetAllMachinesAsync()
        {
            var machines = await _unitOfWork.Machines.GetAllAsync();
            return machines.Select(MapToViewModel).OrderBy(m => m.MachineName);
        }

        public async Task<IEnumerable<MachineViewModel>> GetActiveMachinesAsync()
        {
            var machines = await _unitOfWork.Machines.GetActiveMachinesAsync();
            return machines.Select(MapToViewModel);
        }

        public async Task<IEnumerable<MachineViewModel>> SearchMachinesAsync(string searchTerm)
        {
            var machines = await _unitOfWork.Machines.GetAllAsync();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return machines.Select(MapToViewModel).OrderBy(m => m.MachineName);
            }

            searchTerm = searchTerm.ToLower().Trim();

            var filteredMachines = machines.Where(m =>
                m.MachineName.ToLower().Contains(searchTerm) ||
                (!string.IsNullOrEmpty(m.ModelNumber) && m.ModelNumber.ToLower().Contains(searchTerm)) ||
                (!string.IsNullOrEmpty(m.Manufacturer) && m.Manufacturer.ToLower().Contains(searchTerm)) ||
                m.MaxWidth.ToString().Contains(searchTerm)
            );

            return filteredMachines.Select(MapToViewModel).OrderBy(m => m.MachineName);
        }

        public async Task<MachineViewModel> GetMachineByIdAsync(int id)
        {
            var machine = await _unitOfWork.Machines.GetByIdAsync(id);
            return machine != null ? MapToViewModel(machine) : null;
        }

        public async Task<(bool Success, string[] Errors)> CreateMachineAsync(MachineViewModel model)
        {
            try
            {
                // Check if machine name already exists
                if (await _unitOfWork.Machines.MachineNameExistsAsync(model.MachineName))
                {
                    return (false, new[] { "A machine with this name already exists" });
                }

                var machine = new Machine
                {
                    MachineName = model.MachineName,
                    MaxWidth = model.MaxWidth,
                    Description = model.Description,
                    ModelNumber = model.ModelNumber,
                    Manufacturer = model.Manufacturer,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };

                await _unitOfWork.Machines.AddAsync(machine);
                await _unitOfWork.CompleteAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Error creating machine: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string[] Errors)> UpdateMachineAsync(MachineViewModel model)
        {
            try
            {
                var machine = await _unitOfWork.Machines.GetByIdAsync(model.Id);
                if (machine == null)
                {
                    return (false, new[] { "Machine not found" });
                }

                // Check if new name conflicts with existing machine
                if (await _unitOfWork.Machines.MachineNameExistsAsync(model.MachineName, model.Id))
                {
                    return (false, new[] { "A machine with this name already exists" });
                }

                machine.MachineName = model.MachineName;
                machine.MaxWidth = model.MaxWidth;
                machine.Description = model.Description;
                machine.ModelNumber = model.ModelNumber;
                machine.Manufacturer = model.Manufacturer;
                machine.LastModified = DateTime.Now;
                machine.IsActive = model.IsActive;

                _unitOfWork.Machines.Update(machine);
                await _unitOfWork.CompleteAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Error updating machine: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string[] Errors)> DeleteMachineAsync(int id)
        {
            try
            {
                var machine = await _unitOfWork.Machines.GetByIdAsync(id);
                if (machine == null)
                {
                    return (false, new[] { "Machine not found" });
                }

                _unitOfWork.Machines.Delete(machine);
                await _unitOfWork.CompleteAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Error deleting machine: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string[] Errors)> ToggleMachineStatusAsync(int id)
        {
            try
            {
                var machine = await _unitOfWork.Machines.GetByIdAsync(id);
                if (machine == null)
                {
                    return (false, new[] { "Machine not found" });
                }

                machine.IsActive = !machine.IsActive;
                machine.LastModified = DateTime.Now;

                _unitOfWork.Machines.Update(machine);
                await _unitOfWork.CompleteAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, new[] { $"Error toggling machine status: {ex.Message}" });
            }
        }

        private MachineViewModel MapToViewModel(Machine machine)
        {
            return new MachineViewModel
            {
                Id = machine.Id,
                MachineName = machine.MachineName,
                MaxWidth = machine.MaxWidth,
                Description = machine.Description,
                ModelNumber = machine.ModelNumber,
                Manufacturer = machine.Manufacturer,
                CreatedDate = machine.CreatedDate,
                LastModified = machine.LastModified,
                IsActive = machine.IsActive
            };
        }
    }
}