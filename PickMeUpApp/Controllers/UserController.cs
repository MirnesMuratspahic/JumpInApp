using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PickMeUpApp.Models;
using PickMeUpApp.Models.DTO;
using PickMeUpApp.Services.Interfaces;

namespace PickMeUpApp.Controllers
{
   
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public UserController(IUserService _userService, IHttpContextAccessor _httpContextAccessor)
        {
            userService = _userService;
            httpContextAccessor = _httpContextAccessor;
        }

        [HttpGet("GetUsers-ADMIN")]
        public async Task<IActionResult> GetUsers()
        {
            var (errorStatus, users) = await userService.GetUsers();
            if (errorStatus.Status == true)
                return BadRequest(errorStatus.Name);
            return Ok(users);
        }

        [Authorize]
        [HttpPost("GetUserByEmail")]
        public async Task<IActionResult> GetUserByEmail([FromBody]string email)
        {
            var (errorStatus, user) = await userService.GetUserByEmail(email);
            if(errorStatus.Status == true)
                return BadRequest(errorStatus.Name);
            return Ok(user);
        }

        [Authorize]
        [HttpGet("GetUserRoutes")]
        public async Task<IActionResult> GetUserRoutes ()
        {
            var (errorStatus, routes) = await userService.GetUserRoutes(httpContextAccessor.HttpContext);
            if (errorStatus.Status == true)
                return BadRequest(errorStatus.Name);
            return Ok(routes);
        }

        [HttpPost("UserRegistration")]
        public async Task<IActionResult> UserRegistration(dtoUserRegistration userDto)
        {
            var (errorStatus, token) = await userService.UserRegistration(userDto, httpContextAccessor.HttpContext);
            if (errorStatus.Status == true)
                return BadRequest(errorStatus.Name);
            return Ok(token);
        }

        [HttpPost("UserLogin")]
        public async Task<IActionResult> UserLogin(dtoUserLogin userDto)
        {
            var (errorStatus, token) = await userService.UserLogin(userDto, httpContextAccessor.HttpContext);
            if (errorStatus.Status == true)
                return BadRequest(errorStatus.Name);
            return Ok(token);
        }


    }
}
