using Domain.Entities;
using Domain.Repositories;
using System;
using System.Collections.Generic;
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
}
