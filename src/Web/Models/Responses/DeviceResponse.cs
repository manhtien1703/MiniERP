using Domain.Entities;

namespace Web.Models.Responses;

public class DeviceResponse
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DeviceType DeviceType { get; set; }
    public bool Status { get; set; }
    public string? ImageUrl { get; set; }
    public string WarehouseId { get; set; } = string.Empty;
    public string? WarehouseName { get; set; }
}

