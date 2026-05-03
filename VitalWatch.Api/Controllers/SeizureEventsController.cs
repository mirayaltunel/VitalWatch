using Microsoft.AspNetCore.Mvc;
using VitalWatch.Api.Services.Abstract;

namespace VitalWatch.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeizureEventsController : BaseController
    {
        private readonly ISeizureEventService _seizureEventService;

        public SeizureEventsController(ISeizureEventService seizureEventService)
        {
            _seizureEventService = seizureEventService;
        }
    }
}
