using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Repositories;

public interface IDeviceRepository
{
    Task<IEnumerable<CoolingDevice>> GetAllAsync();
    Task<CoolingDevice?> GetByIdAsync(string id);
    Task AddAsync(CoolingDevice device);
    Task UpdateAsync(CoolingDevice device);
    Task DeleteAsync(CoolingDevice device);
    Task<int> CountByWarehouseAsync(string warehouseId);
}
