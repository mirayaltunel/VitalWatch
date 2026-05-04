using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VitalWatch.Api.Models.Requests;
using VitalWatch.Api.Services.Abstract;

namespace VitalWatch.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult>Register ([FromBody] RegisterRequestModel model)
        {
            var resp = await _userService.Register(model);
            return HandleResponse(resp);
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login ([FromBody] LoginRequestModel model)
        {
            var resp = await _userService.Login(model);
            return HandleResponse(resp);
        }
    }
}
