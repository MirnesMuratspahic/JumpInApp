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

        [HttpGet("GetRoutes")]
        public async Task<IActionResult> GetRoutes()
        {
            var (errorStatus, routes) = await userService.GetRoutes();
            if (errorStatus.Status == true)
                return BadRequest(errorStatus.Name);
            return Ok(routes);
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

        [HttpPost("AcceptOrDeclineRequest/{choise}")]
        public async Task<IActionResult> AcceptOrDeclineRequest([FromRoute] int choise, dtoRequest request)
        {
            var (errorStatus, acceptedRequests) = await userService.AcceptOrDeclineRequest(choise, request);
            if (errorStatus.Status == true)
                return BadRequest(errorStatus.Name);
            return Ok(acceptedRequests);
        }

        [HttpPost("GetRecivedRequests")]
        public async Task<IActionResult> GetRecivedRequests([FromBody] string email)
        {
            var (errorStatus, recivedRequests) = await userService.GetRecivedRequests(email);
            if (errorStatus.Status == true)
                return BadRequest(errorStatus.Name);
            return Ok(recivedRequests);
        }


    }
}
