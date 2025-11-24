using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Repositories;

public interface IDeviceLogRepository
{
    Task AddLogAsync(DeviceLog log);
    Task<DeviceLog?> GetLatestLogAsync(string deviceId);
    Task<IEnumerable<DeviceLog>> GetHistoryAsync(string deviceId, DateTime from, DateTime to);
}
