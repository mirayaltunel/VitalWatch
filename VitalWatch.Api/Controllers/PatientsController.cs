using Microsoft.AspNetCore.Mvc;
using VitalWatch.Api.Models.Requests;
using VitalWatch.Api.Services.Abstract;

namespace VitalWatch.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : BaseController
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddPatient([FromBody] AddPatientRequestModel model, [FromQuery] int userId)
        {
            var resp = await _patientService.AddPatient(model, userId);
            return HandleResponse(resp);
        }

        [HttpGet("MyPatients/{userId}")]
        public async Task<IActionResult> GetMyPatients(int userId)
        {
            var resp = await _patientService.GetMyPatients(userId);
            return HandleResponse(resp);
        }
    }
}
