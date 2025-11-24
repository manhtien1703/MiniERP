using Domain.Entities;
using Web.Models.Responses;

namespace Web.Mappers;

public static class DeviceMapper
{
    public static DeviceResponse ToResponse(this CoolingDevice device)
    {
        return new DeviceResponse
        {
            Id = device.Id,
            Name = device.Name,
            DeviceType = device.DeviceType,
            Status = device.Status,
            ImageUrl = device.ImageUrl,
            WarehouseId = device.WarehouseId,
            WarehouseName = device.Warehouse?.Name
        };
    }
}

