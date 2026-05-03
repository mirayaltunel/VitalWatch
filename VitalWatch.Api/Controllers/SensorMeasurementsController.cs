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
    }
}
