using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PickMeUpApp.Models;
using PickMeUpApp.Models.DTO;
using PickMeUpApp.Services.Interfaces;

namespace PickMeUpApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RouteController : ControllerBase
    {
        private readonly IRouteService userService;
        public RouteController(IRouteService _routeService)
        {
            userService = _routeService;
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
        public async Task<IActionResult> AddRoute(dtoUserRoute dtoRoute)
        {
            var (errorStatus, route) = await userService.AddRoute(dtoRoute);
            if (errorStatus.Status == true)
                return BadRequest(errorStatus.Name);
            return Ok(route);
        }
    }
}
