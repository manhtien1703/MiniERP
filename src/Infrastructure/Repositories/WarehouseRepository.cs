using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly MiniErpDbContext _db;

    public WarehouseRepository(MiniErpDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Warehouse>> GetAllAsync()
    {
        return await _db.Warehouses.Include(w => w.Province).ToListAsync();
    }

    public async Task<Warehouse?> GetByIdAsync(string id)
    {
        return await _db.Warehouses.Include(w => w.Province)
                                   .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task AddAsync(Warehouse warehouse)
    {
        _db.Warehouses.Add(warehouse);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id) 
    {
        var warehouse = await _db.Warehouses.FindAsync(id);
        if (warehouse != null)
        {
            _db.Warehouses.Remove(warehouse);
            await _db.SaveChangesAsync();
        }
    }

    public async Task UpdateAsync(Warehouse warehouse)
    {
        _db.Warehouses.Update(warehouse);
        await _db.SaveChangesAsync();
    }

    public async Task<int> CountByProvinceAsync(int provinceId)
    {
        return await _db.Warehouses.CountAsync(w => w.ProvinceId == provinceId);
    }
}
