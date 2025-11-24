using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repositories;

public class DeviceRepository : IDeviceRepository
{
    private readonly MiniErpDbContext _db;

    public DeviceRepository(MiniErpDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<CoolingDevice>> GetAllAsync() =>
        await _db.Devices.Include(d => d.Warehouse).ToListAsync();

    public async Task<CoolingDevice?> GetByIdAsync(string id) =>
        await _db.Devices.Include(d => d.Warehouse)
                         .FirstOrDefaultAsync(d => d.Id == id);

    public async Task AddAsync(CoolingDevice device)
    {
        _db.Devices.Add(device);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(CoolingDevice device)
    {
        _db.Devices.Update(device);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(CoolingDevice device)
    {
        _db.Devices.Remove(device);
        await _db.SaveChangesAsync();
    }

    public async Task<int> CountByWarehouseAsync(string warehouseId)
    {
        return await _db.Devices.CountAsync(d => d.WarehouseId == warehouseId);
    }
}
