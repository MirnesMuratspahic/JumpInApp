using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PickMeUpApp.Services.Interfaces;
using PickMeUpApp.Services;

namespace PickMeUpApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BaseController : ControllerBase
    {
        public IHttpContextAccessor httpContextAccessor;
        public BaseController(IHttpContextAccessor _httpContextAccessor)
        {
            httpContextAccessor = _httpContextAccessor;
        }
    }
}
