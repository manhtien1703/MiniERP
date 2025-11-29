using Domain.Entities;
using Domain.Repositories;
using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services;

public class MonitoringService
{
    private readonly IDeviceLogRepository _repo;

    public MonitoringService(IDeviceLogRepository repo)
    {
        _repo = repo;
    }

    public async Task<DeviceLog?> GetLatest(string deviceId) => await _repo.GetLatestLogAsync(deviceId);

    public async Task<IEnumerable<DeviceLog>> GetHistory(string deviceId, DateTime from, DateTime to)
        => await _repo.GetHistoryAsync(deviceId, from, to);

    public async Task<IEnumerable<AggregatedLogDto>> GetAggregatedHistory(
        string deviceId, 
        DateTime from, 
        DateTime to, 
        TimeSpan interval)
    {
        var logs = await _repo.GetHistoryAsync(deviceId, from, to);
        var logsList = logs.ToList();

        if (!logsList.Any())
            return Enumerable.Empty<AggregatedLogDto>();

        // Nhóm logs theo interval
        var grouped = logsList
            .GroupBy(log => GetIntervalStart(log.Timestamp, interval))
            .Select(group => new AggregatedLogDto
            {
                Timestamp = group.Key,
                Temperature = new AggregatedValueDto
                {
                    Avg = group.Average(l => l.Temperature),
                    Min = group.Min(l => l.Temperature),
                    Max = group.Max(l => l.Temperature)
                },
                Humidity = new AggregatedValueDto
                {
                    Avg = group.Average(l => l.Humidity),
                    Min = group.Min(l => l.Humidity),
                    Max = group.Max(l => l.Humidity)
                },
                Count = group.Count()
            })
            .OrderBy(x => x.Timestamp)
            .ToList();

        // Nếu có ít dữ liệu, có thể cần fill các khoảng trống để biểu đồ hiển thị tốt hơn
        // Nhưng tạm thời trả về dữ liệu thực tế có được

        return grouped;
    }

    private DateTime GetIntervalStart(DateTime timestamp, TimeSpan interval)
    {
        // Đảm bảo timestamp là UTC
        var utcTimestamp = timestamp.Kind == DateTimeKind.Utc 
            ? timestamp 
            : timestamp.ToUniversalTime();
        
        // Tính toán interval start bằng cách làm tròn xuống
        var ticks = utcTimestamp.Ticks / interval.Ticks;
        return new DateTime(ticks * interval.Ticks, DateTimeKind.Utc);
    }
}
