using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using Web.Models;
using Web.Models.Responses;
using Web.Mappers;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Yêu cầu authentication cho tất cả endpoints
public class WarehouseController : ControllerBase
{
    private readonly WarehouseService _service;

    public WarehouseController(WarehouseService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var warehouses = await _service.GetAllAsync();
        var result = warehouses.Select(w => w.ToResponse());
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWarehouseRequest request)
    {
        var warehouse = await _service.CreateAsync(
            request.Name,
            request.Location,
            request.Capacity,
            request.ProvinceId
        );

        return CreatedAtAction(nameof(GetById), new { id = warehouse.Id }, warehouse.ToResponse());
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var warehouse = await _service.GetByIdAsync(id);
        if (warehouse == null)
            return NotFound(new ErrorResponse 
            { 
                Title = "Not Found", 
                Status = 404, 
                Detail = "Warehouse not found" 
            });

        return Ok(warehouse.ToResponse());
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success)
            return NotFound(new ErrorResponse 
            { 
                Title = "Not Found", 
                Status = 404, 
                Detail = "Warehouse not found" 
            });

        return NoContent();
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateWarehouseRequest request)
    {
        var updated = await _service.UpdateAsync(
            id,
            request.Name,
            request.Location,
            request.Capacity,
            request.ProvinceId
        );

        if (updated == null)
            return NotFound(new ErrorResponse 
            { 
                Title = "Not Found", 
                Status = 404, 
                Detail = "Warehouse not found" 
            });

        return Ok(updated.ToResponse());
    }
}
