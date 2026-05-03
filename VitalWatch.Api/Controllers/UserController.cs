using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VitalWatch.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public UserController()
        {
            
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("test");
        }
    }
}
