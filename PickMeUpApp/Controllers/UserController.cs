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

        public UserController(IUserService _userService)
        {
            userService = _userService;

        }

        [HttpPost("UserRegistration")]
        public async Task<IActionResult> UserRegistration(dtoUserRegistration userDto)
        {
            var (errorStatus, token) = await userService.UserRegistration(userDto);
            if (errorStatus.Status == true)
                return BadRequest(errorStatus.Name);
            return Ok(token);
        }

        [HttpPost("UserLogin")]
        public async Task<IActionResult> UserLogin(dtoUserLogin userDto)
        {
            var (errorStatus, token) = await userService.UserLogin(userDto);
            if (errorStatus.Status == true)
                return BadRequest(errorStatus.Name);
            return Ok(token);
        }

        [HttpPost("AddRoute")]
        public async Task<IActionResult> AddRoute(dtoUserRoute dtoRoute)
        {
            var (errorStatus, route) = await userService.AddRoute (dtoRoute);
            if (errorStatus.Status == true)
                return BadRequest(errorStatus.Name);
            return Ok(route);
        }
    }
}
