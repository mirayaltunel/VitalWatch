using Microsoft.AspNetCore.Mvc;
using VitalWatch.Api.Services.Abstract;

namespace VitalWatch.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlertsController : BaseController
    {
        private readonly IAlertService _service;
        public AlertsController(IAlertService service) { _service = service; }

        [HttpGet("Patient/{patientId}")]
        public async Task<IActionResult> GetLatest(int patientId, [FromQuery] int take = 20)
            => HandleResponse(await _service.GetLatestAlerts(patientId, take));

        [HttpPut("{alertId}/Review")]
        public async Task<IActionResult> Review(int alertId)
            => HandleResponse(await _service.MarkReviewed(alertId));
    }
}
