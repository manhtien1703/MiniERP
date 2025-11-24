using Domain.Entities;
using Domain.Repositories;
using Domain.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services;

public class WarehouseService
{
    private readonly IWarehouseRepository _repo;
    private readonly IProvinceRepository _provinceRepo;

    public WarehouseService(IWarehouseRepository repo, IProvinceRepository provinceRepo)
    {
        _repo = repo;
        _provinceRepo = provinceRepo;
    }

    public async Task<IEnumerable<Warehouse>> GetAllAsync() => await _repo.GetAllAsync();

    public async Task<Warehouse?> GetByIdAsync(string id) => await _repo.GetByIdAsync(id);

    public async Task<Warehouse> CreateAsync(string name, string location, int capacity, int provinceId)
    {
        var province = await _provinceRepo.GetByIdAsync(provinceId);
        if (province == null)
            throw new NotFoundException(nameof(Province), provinceId);

        var next = await _repo.CountByProvinceAsync(provinceId) + 1;
        var warehouseId = $"{province.Code}{next:D3}"; 

        var warehouse = new Warehouse
        {
            Id = warehouseId,
            Name = name,
            Location = location,
            Capacity = capacity,
            ProvinceId = provinceId
        };

        await _repo.AddAsync(warehouse);
        return warehouse;
    }
    public async Task<bool> DeleteAsync(string id)
    {
        var warehouse = await _repo.GetByIdAsync(id);
        if (warehouse == null)
            return false;

        await _repo.DeleteAsync(id);
        return true;
    }

    public async Task<Warehouse?> UpdateAsync(
    string id,
    string name,
    string location,
    int capacity,
    int provinceId)
    {
        var warehouse = await _repo.GetByIdAsync(id);
        if (warehouse == null)
            return null;

        var province = await _provinceRepo.GetByIdAsync(provinceId);
        if (province == null)
            throw new NotFoundException(nameof(Province), provinceId);

        warehouse.Name = name;
        warehouse.Location = location;
        warehouse.Capacity = capacity;
        warehouse.ProvinceId = provinceId;

        await _repo.UpdateAsync(warehouse);
        return warehouse;
    }

}
