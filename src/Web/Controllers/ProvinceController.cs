using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProvinceController : ControllerBase
    {
        private readonly ProvinceService _service;

        public ProvinceController(ProvinceService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Province>>> GetAll()
        {
            var provinces = await _service.GetAllAsync();
            return Ok(provinces);
        }
    }
}
