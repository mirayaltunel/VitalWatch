using Microsoft.AspNetCore.Mvc;
using VitalWatch.Api.Services.Abstract;

namespace VitalWatch.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiseasesController : BaseController
    {
        private readonly IDiseaseService _service;
        public DiseasesController(IDiseaseService service) { _service = service; }

        [HttpGet]
        public async Task<IActionResult> GetAll() => HandleResponse(await _service.GetAll());
    }
}
