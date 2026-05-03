using Microsoft.AspNetCore.Mvc;
using VitalWatch.Api.Services.Abstract;

namespace VitalWatch.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientDiseasesController : BaseController
    {
        private readonly IPatientDiseaseService _patientDiseaseService;

        public PatientDiseasesController(IPatientDiseaseService patientDiseaseService)
        {
            _patientDiseaseService = patientDiseaseService;
        }
    }
}
