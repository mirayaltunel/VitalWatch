using Microsoft.AspNetCore.Mvc;
using VitalWatch.Api.Services.Abstract;

namespace VitalWatch.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthEventsController : BaseController
    {
        private readonly IHealthEventService _healthEventService;

        public HealthEventsController(IHealthEventService healthEventService)
        {
            _healthEventService = healthEventService;
        }

        [HttpGet("Patient/{patientId}/Alerts")]
        public async Task<IActionResult> GetLatestAlerts(int patientId)
        {
            var resp = await _healthEventService.GetLatestAlerts(patientId);
            return HandleResponse(resp);
        }

        [HttpGet("Patient/{patientId}/Reports")]
        public async Task<IActionResult> GetPatientReports(int patientId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var resp = await _healthEventService.GetPatientReports(patientId, startDate, endDate);
            return HandleResponse(resp);
        }
    }
}
