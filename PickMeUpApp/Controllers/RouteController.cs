using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PickMeUpApp.Models;
using PickMeUpApp.Models.DTO;
using PickMeUpApp.Services.Interfaces;

namespace PickMeUpApp.Controllers
{
    [Authorize]
    public class RouteController : BaseController
    {
        private readonly IRouteService _routeService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public RouteController(IRouteService routeService, IHttpContextAccessor _httpContextAccessor) : base(_httpContextAccessor)
        {
            _routeService = routeService;   
        }

        [HttpGet("GetRoutes")]
        public async Task<IActionResult> GetRoutes()
        {
            var (errorStatus, routes) = await _routeService.GetRoutes();
            if (errorStatus.Status == true)
                return BadRequest(errorStatus.Name);
            return Ok(routes);
        }

        [HttpPost("AddRoute")]
        public async Task<IActionResult> AddRoute(UserRoute dtoRoute)
        {
            var (errorStatus, route) = await _routeService.AddRoute(dtoRoute, httpContextAccessor.HttpContext);
            if (errorStatus.Status == true)
                return BadRequest(errorStatus.Name);
            return Ok(route);
        }
    }
}
