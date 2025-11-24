using Domain.Entities;

namespace Web.Models;

public class UpdateDeviceRequest
{
    public string Name { get; set; } = string.Empty;
    public DeviceType DeviceType { get; set; }
    public bool Status { get; set; }
    public string? ImageUrl { get; set; }
}

