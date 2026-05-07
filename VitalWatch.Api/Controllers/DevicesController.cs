using Microsoft.AspNetCore.Mvc;
using VitalWatch.Api.Models.Requests;
using VitalWatch.Api.Services.Abstract;

namespace VitalWatch.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController : BaseController
    {
        private readonly IDeviceService _service;

        public DevicesController(IDeviceService service) { _service = service; }

        [HttpGet("Patient/{patientId}")]
        public async Task<IActionResult> GetPatientDevices(int patientId)
            => HandleResponse(await _service.GetPatientDevices(patientId));

        [HttpGet("User/{userId}")]
        public async Task<IActionResult> GetUserDevices(int userId)
            => HandleResponse(await _service.GetUserDevices(userId));

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] AddDeviceRequestModel model)
            => HandleResponse(await _service.AddDevice(model));

        [HttpPut("{deviceId}/Status")]
        public async Task<IActionResult> UpdateStatus(int deviceId, [FromQuery] int statusId, [FromQuery] int? batteryLevel)
            => HandleResponse(await _service.UpdateStatus(deviceId, statusId, batteryLevel));

        [HttpDelete("{deviceId}")]
        public async Task<IActionResult> Delete(int deviceId)
            => HandleResponse(await _service.Delete(deviceId));
    }
}
