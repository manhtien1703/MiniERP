using Domain.Entities;
using Domain.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ProvinceService
    {
        private readonly IProvinceRepository _repo;

        public ProvinceService(IProvinceRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Province>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }
    }
}
