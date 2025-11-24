using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Web.Models;
using Web.Models.Responses;
using Web.Mappers;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Yêu cầu authentication cho tất cả endpoints
public class DeviceController : ControllerBase
{
    private readonly DeviceService _service;

    public DeviceController(DeviceService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var devices = await _service.GetAllAsync();
        var result = devices.Select(d => d.ToResponse());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var device = await _service.GetByIdAsync(id);
        if (device == null)
            return NotFound(new ErrorResponse 
            { 
                Title = "Not Found", 
                Status = 404, 
                Detail = "Device not found" 
            });

        return Ok(device.ToResponse());
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDeviceRequest req)
    {
        var device = await _service.CreateAsync(req.Name, req.DeviceType, req.WarehouseId, req.ImageUrl);
        return CreatedAtAction(nameof(GetById), new { id = device.Id }, device.ToResponse());
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateDeviceRequest req)
    {
        var updated = await _service.UpdateAsync(id, req.Name, req.DeviceType, req.Status, req.ImageUrl);
        if (updated == null)
            return NotFound(new ErrorResponse 
            { 
                Title = "Not Found", 
                Status = 404, 
                Detail = "Device not found" 
            });

        return Ok(updated.ToResponse());
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
            return NotFound(new ErrorResponse 
            { 
                Title = "Not Found", 
                Status = 404, 
                Detail = "Device not found" 
            });

        return NoContent();
    }
}
