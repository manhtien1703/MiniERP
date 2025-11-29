using System;

namespace Application.DTOs;

public class AggregatedLogDto
{
    public DateTime Timestamp { get; set; }
    public AggregatedValueDto Temperature { get; set; } = new();
    public AggregatedValueDto Humidity { get; set; } = new();
    public int Count { get; set; }
}

public class AggregatedValueDto
{
    public double Avg { get; set; }
    public double Min { get; set; }
    public double Max { get; set; }
}

