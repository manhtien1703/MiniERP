using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories;

public class DeviceLogRepository : IDeviceLogRepository
{
    private readonly MiniErpDbContext _context;
    public DeviceLogRepository(MiniErpDbContext context) => _context = context;

    public async Task AddLogAsync(DeviceLog log)
    {
        await _context.DeviceLogs.AddAsync(log);
        await _context.SaveChangesAsync();
    }

    public async Task<DeviceLog?> GetLatestLogAsync(string deviceId)
        => await _context.DeviceLogs
            .Where(l => l.DeviceId == deviceId)
            .OrderByDescending(l => l.Timestamp)
            .FirstOrDefaultAsync();

    public async Task<IEnumerable<DeviceLog>> GetHistoryAsync(string deviceId, DateTime from, DateTime to)
        => await _context.DeviceLogs
            .Where(l => l.DeviceId == deviceId && l.Timestamp >= from && l.Timestamp <= to)
            .OrderBy(l => l.Timestamp)
            .ToListAsync();

    public async Task DeleteLogsByDeviceIdAsync(string deviceId)
    {
        var logs = await _context.DeviceLogs
            .Where(l => l.DeviceId == deviceId)
            .ToListAsync();
        
        _context.DeviceLogs.RemoveRange(logs);
        await _context.SaveChangesAsync();
    }
}
