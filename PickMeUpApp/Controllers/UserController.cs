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

        [HttpGet("GetUsers-ADMIN")]
        public async Task<IActionResult> GetUsers()
        {
            var (errorStatus, users) = await userService.GetUsers();
            if (errorStatus.Status == true)
                return BadRequest(errorStatus.Name);
            return Ok(users);
        }

        [HttpPost("GetUserRoutes")]
        public async Task<IActionResult> GetUserRoutes ([FromBody] string email)
        {
            var (errorStatus, routes) = await userService.GetUserRoutes(email);
            if (errorStatus.Status == true)
                return BadRequest(errorStatus.Name);
            return Ok(routes);
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

        [HttpPost("SendRequest")]
        public async Task<IActionResult> SendRequest(dtoRequest dtoRequest)
        {
            var (errorStatus, request) = await userService.SendRequest(dtoRequest);
            if (errorStatus.Status == true)
                return BadRequest(errorStatus.Name);
            return Ok(request);
        }

        [HttpPost("GetSentRequests")]
        public async Task<IActionResult> GetSentRequests([FromBody] string email)
        {
            var (errorStatus, sentRequests) = await userService.GetSentRequests(email);
            if (errorStatus.Status == true)
                return BadRequest(errorStatus.Name);
            return Ok(sentRequests);
        }


    }
}
