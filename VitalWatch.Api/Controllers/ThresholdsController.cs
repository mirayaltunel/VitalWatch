using Microsoft.AspNetCore.Mvc;
using VitalWatch.Api.Models.Requests;
using VitalWatch.Api.Services.Abstract;

namespace VitalWatch.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThresholdsController : BaseController
    {
        private readonly IThresholdService _service;
        public ThresholdsController(IThresholdService service) { _service = service; }

        [HttpGet("Patient/{patientId}")]
        public async Task<IActionResult> GetByPatient(int patientId)
            => HandleResponse(await _service.GetByPatient(patientId));

        [HttpPost("Upsert")]
        public async Task<IActionResult> Upsert([FromBody] SetThresholdRequestModel model)
            => HandleResponse(await _service.Upsert(model));

        [HttpDelete("{thresholdId}")]
        public async Task<IActionResult> Delete(int thresholdId)
            => HandleResponse(await _service.Delete(thresholdId));
    }
}
