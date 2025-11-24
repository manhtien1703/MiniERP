using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Repositories;

public interface IWarehouseRepository
{
    Task<IEnumerable<Warehouse>> GetAllAsync();
    Task<Warehouse?> GetByIdAsync(string id);
    Task AddAsync(Warehouse warehouse);
    Task<int> CountByProvinceAsync(int provinceId);
    Task DeleteAsync(string id);
    Task UpdateAsync(Warehouse warehouse);
}
