using Microsoft.AspNetCore.Mvc;
using VitalWatch.Api.Services.Abstract;

namespace VitalWatch.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorMeasurementsController : BaseController
    {
        private readonly ISensorMeasurementService _sensorMeasurementService;

        public SensorMeasurementsController(ISensorMeasurementService sensorMeasurementService)
        {
            _sensorMeasurementService = sensorMeasurementService;
        }

        [HttpGet("Patient/{patientId}/Latest")]
        public async Task<IActionResult> GetLatestVitals(int patientId)
        {
            var resp = await _sensorMeasurementService.GetLatestVitals(patientId);
            return HandleResponse(resp);
        }

        [HttpGet("Patient/{patientId}/History")]
        public async Task<IActionResult> GetVitalHistory(int patientId, [FromQuery] VitalWatch.Api.Enums.MeasurementType type)
        {
            var resp = await _sensorMeasurementService.GetVitalHistory(patientId, type);
            return HandleResponse(resp);
        }
    }
}
