using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalWatch.Api.ResponseManage;

namespace VitalWatch.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BaseController : ControllerBase
    {
        protected IActionResult HandleResponse(ResponseModel responseModel)
        {
            if (responseModel.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return Ok(responseModel);
            }
            else if (responseModel.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return BadRequest(responseModel);
            }
            else
                return StatusCode(500, responseModel);
        }

        protected IActionResult HandleResponse<T>(ResponseModel<T> responseModel)
        {
            if (responseModel.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return Ok(responseModel);
            }
            else if (responseModel.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return BadRequest(responseModel);
            }
            else
                return StatusCode(500, responseModel);
        }
    }
}
