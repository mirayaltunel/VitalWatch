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
    }
}
