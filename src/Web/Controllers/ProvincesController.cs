using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProvincesController : ControllerBase
{
    private readonly ProvinceService _service;

    public ProvincesController(ProvinceService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var provinces = await _service.GetAllAsync();
        return Ok(provinces);
    }
}

