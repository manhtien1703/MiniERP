using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Repositories;

public interface IProvinceRepository
{
    Task<IEnumerable<Province>> GetAllAsync();
    Task<Province?> GetByIdAsync(int id);
}
