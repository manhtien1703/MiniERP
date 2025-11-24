using System;

namespace Domain.Entities;

public class DeviceLog
{
    public int Id { get; set; } 
    public string DeviceId { get; set; } = string.Empty;
    public CoolingDevice? Device { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public double Temperature { get; set; }
    public double Humidity { get; set; }
}
