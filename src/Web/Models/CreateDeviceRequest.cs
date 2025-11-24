using Domain.Entities;

namespace Web.Models;

public class CreateDeviceRequest
{
    public string Name { get; set; } = string.Empty;
    public DeviceType DeviceType { get; set; }
    public string WarehouseId { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}

