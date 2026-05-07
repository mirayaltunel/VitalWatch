using Microsoft.AspNetCore.Mvc;
using VitalWatch.Api.Services.Abstract;

namespace VitalWatch.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorMeasurementsController : BaseController
    {
        private readonly ISensorMeasurementService _service;

        public SensorMeasurementsController(ISensorMeasurementService service)
        {
            _service = service;
        }

        [HttpGet("Patient/{patientId}/Latest")]
        public async Task<IActionResult> GetLatestVitals(int patientId)
            => HandleResponse(await _service.GetLatestVitals(patientId));

        [HttpGet("Patient/{patientId}/History")]
        public async Task<IActionResult> GetVitalHistory(int patientId, [FromQuery] int measurementTypeId, [FromQuery] int take = 50)
            => HandleResponse(await _service.GetVitalHistory(patientId, measurementTypeId, take));
    }
}
