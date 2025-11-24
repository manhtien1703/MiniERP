using System;
using System.Collections.Generic;

namespace Domain.Entities;

public enum DeviceType
{
    Cooler,        // Làm mát
    Freezer,       // Làm đông
    Dehumidifier   // Hút ẩm
}

public class CoolingDevice
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DeviceType DeviceType { get; set; }
    public bool Status { get; set; } = true;
    public string? ImageUrl { get; set; }

    public string WarehouseId { get; set; } = string.Empty;
    public Warehouse? Warehouse { get; set; }

    public ICollection<DeviceLog> Logs { get; set; } = new List<DeviceLog>();
}
