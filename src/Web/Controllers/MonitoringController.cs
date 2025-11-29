using System;
using System.Threading.Tasks;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Yêu cầu authentication cho tất cả endpoints
public class MonitoringController : ControllerBase
{
    private readonly MonitoringService _service;
    public MonitoringController(MonitoringService service) => _service = service;

    [HttpGet("{deviceId}/latest")]
    public async Task<IActionResult> GetLatest(string deviceId)
        => Ok(await _service.GetLatest(deviceId));

    [HttpGet("{deviceId}/history")]
    public async Task<IActionResult> GetHistory(string deviceId, DateTime from, DateTime to)
        => Ok(await _service.GetHistory(deviceId, from, to));

    [HttpGet("{deviceId}/chart")]
    public async Task<IActionResult> GetChartData(string deviceId, [FromQuery] string timeRange = "24h")
    {
        var (from, to, interval) = ParseTimeRange(timeRange);
        var data = await _service.GetAggregatedHistory(deviceId, from, to, interval);
        return Ok(data);
    }

    private (DateTime from, DateTime to, TimeSpan interval) ParseTimeRange(string range)
    {
        var now = DateTime.UtcNow;
        return range.ToLower() switch
        {
            "1h" => (now.AddHours(-1), now, TimeSpan.FromMinutes(5)),
            "6h" => (now.AddHours(-6), now, TimeSpan.FromMinutes(30)),
            "24h" => (now.AddDays(-1), now, TimeSpan.FromHours(1)),
            "7d" => (now.AddDays(-7), now, TimeSpan.FromHours(6)),
            "30d" => (now.AddDays(-30), now, TimeSpan.FromDays(1)),
            _ => (now.AddDays(-1), now, TimeSpan.FromHours(1))
        };
    }
}
