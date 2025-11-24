using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Domain.Entities;
using Domain.Repositories;

namespace Web.Services;

public class FakeSensorService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<FakeSensorService> _logger;
    private readonly Random _rand = new();

    public FakeSensorService(IServiceScopeFactory scopeFactory, ILogger<FakeSensorService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("FakeSensorService is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var deviceRepo = scope.ServiceProvider.GetRequiredService<IDeviceRepository>();
                var logRepo = scope.ServiceProvider.GetRequiredService<IDeviceLogRepository>();

                var devices = await deviceRepo.GetAllAsync();
                
                if (devices == null || !devices.Any())
                {
                    _logger.LogInformation("No devices found. Waiting for devices to be added...");
                    await Task.Delay(10000, stoppingToken);
                    continue;
                }

                foreach (var device in devices)
                {
                    var temp = device.DeviceType switch
                    {
                        DeviceType.Cooler => _rand.Next(2, 10),        // kho lam mat
                        DeviceType.Freezer => _rand.Next(-20, -5),     // kho đang
                        DeviceType.Dehumidifier => _rand.Next(15, 25), // hut am
                        _ => 20
                    };
                    var hum = _rand.Next(40, 90);

                    var log = new DeviceLog
                    {
                        DeviceId = device.Id,
                        Temperature = temp,
                        Humidity = hum,
                        Timestamp = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)
                    };

                    await logRepo.AddLogAsync(log);
                }

                _logger.LogDebug("Generated sensor data for {DeviceCount} devices.", devices.Count());
                await Task.Delay(5000, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in FakeSensorService. Retrying in 10 seconds...");
                await Task.Delay(10000, stoppingToken);
            }
        }

        _logger.LogInformation("FakeSensorService is stopping.");
    }
}
