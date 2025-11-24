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
}
