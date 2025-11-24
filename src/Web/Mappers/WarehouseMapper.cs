using Domain.Entities;
using Web.Models.Responses;

namespace Web.Mappers;

public static class WarehouseMapper
{
    public static WarehouseResponse ToResponse(this Warehouse warehouse)
    {
        return new WarehouseResponse
        {
            Id = warehouse.Id,
            Name = warehouse.Name,
            Location = warehouse.Location,
            Capacity = warehouse.Capacity,
            ProvinceId = warehouse.ProvinceId,
            ProvinceName = warehouse.Province?.Name
        };
    }
}

