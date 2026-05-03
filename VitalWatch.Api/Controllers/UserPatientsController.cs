using Microsoft.AspNetCore.Mvc;
using VitalWatch.Api.Services.Abstract;

namespace VitalWatch.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserPatientsController : BaseController
    {
        private readonly IUserPatientService _userPatientService;

        public UserPatientsController(IUserPatientService userPatientService)
        {
            _userPatientService = userPatientService;
        }
    }
}
