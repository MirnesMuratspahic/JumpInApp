using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PickMeUpApp.Models;
using PickMeUpApp.Models.DTO;
using PickMeUpApp.Services.Interfaces;

namespace PickMeUpApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RouteController : ControllerBase
    {
        private readonly IRouteService userService;
        public IHttpContextAccessor httpContextAccessor;
        public RouteController(IRouteService _routeService, IHttpContextAccessor _httpContextAccessor)
        {
            userService = _routeService;
            httpContextAccessor = _httpContextAccessor;
        }

        [HttpGet("GetRoutes")]
        public async Task<IActionResult> GetRoutes()
        {
            var (errorStatus, routes) = await userService.GetRoutes();
            if (errorStatus.Status == true)
                return BadRequest(errorStatus.Name);
            return Ok(routes);
        }

        [HttpPost("AddRoute")]
        public async Task<IActionResult> AddRoute(UserRoute dtoRoute)
        {
            var (errorStatus, route) = await userService.AddRoute(dtoRoute, httpContextAccessor.HttpContext);
            if (errorStatus.Status == true)
                return BadRequest(errorStatus.Name);
            return Ok(route);
        }
    }
}
