using PrinterApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrinterApp.Data.Repositories;

public interface IRollDirectionRepository : IRepository<RollDirection> 
{
    Task<List<RollDirection>> GetActiveDirectionsAsync();
    Task<RollDirection> GetByDirectionNumberAsync(int directionNumber);
    Task<bool> DirectionNumberExistsAsync(int directionNumber, int? excludeId = null);
}
