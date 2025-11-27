using Domain.Entities;
using Domain.Repositories;
using Domain.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Application.Services;

public class DeviceService
{
    private readonly IDeviceRepository _repo;
    private readonly IWarehouseRepository _warehouseRepo;
    private readonly IDeviceLogRepository _logRepo;

    public DeviceService(IDeviceRepository repo, IWarehouseRepository warehouseRepo, IDeviceLogRepository logRepo)
    {
        _repo = repo;
        _warehouseRepo = warehouseRepo;
        _logRepo = logRepo;
    }

    public async Task<IEnumerable<CoolingDevice>> GetAllAsync() => await _repo.GetAllAsync();

    public async Task<CoolingDevice?> GetByIdAsync(string id) => await _repo.GetByIdAsync(id);

    public async Task<CoolingDevice> CreateAsync(string name, DeviceType type, string warehouseId, string? imageUrl = null)
    {
        var warehouse = await _warehouseRepo.GetByIdAsync(warehouseId);
        if (warehouse == null)
            throw new NotFoundException(nameof(Warehouse), warehouseId);

        var deviceCount = await _repo.CountByWarehouseAsync(warehouseId);
        var next = deviceCount + 1;
        var deviceId = $"{warehouseId}-{next:D3}";

        var device = new CoolingDevice
        {
            Id = deviceId,
            Name = name,
            DeviceType = type,
            Status = true,
            ImageUrl = imageUrl,
            WarehouseId = warehouseId
        };

        await _repo.AddAsync(device);
        return device;
    }

    public async Task<CoolingDevice?> UpdateAsync(string id, string name, DeviceType type, bool status, string? imageUrl = null)
    {
        var device = await _repo.GetByIdAsync(id);
        if (device == null)
            return null;

        device.Name = name;
        device.DeviceType = type;
        device.Status = status;
        device.ImageUrl = imageUrl;

        await _repo.UpdateAsync(device);
        return device;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var device = await _repo.GetByIdAsync(id);
        if (device == null)
            return false;

        // Xóa tất cả DeviceLog liên quan trước khi xóa Device
        await _logRepo.DeleteLogsByDeviceIdAsync(id);
        
        await _repo.DeleteAsync(device);
        return true;
    }
}
