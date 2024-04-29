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
    public class RequestController : ControllerBase
    {
        public IRequestService requestService { get; set; }
        public IHttpContextAccessor httpContextAccessor;
        public RequestController(IRequestService _requestService, IHttpContextAccessor _httpContextAccessor) 
        {
            requestService = _requestService;
            httpContextAccessor = _httpContextAccessor;
        }

        [HttpPost("SendRequest")]
        public async Task<IActionResult> SendRequest(dtoRequestSent dtoRequest)
        {
            var (errorStatus, request) = await requestService.SendRequest(dtoRequest, httpContextAccessor.HttpContext);
            if (errorStatus.Status == true)
                return BadRequest(errorStatus.Name);
            return Ok(request);
        }

        [HttpGet("GetSentRequests")]
        public async Task<IActionResult> GetSentRequests()
        {
            var (errorStatus, sentRequests) = await requestService.GetSentRequests(httpContextAccessor.HttpContext);
            if (errorStatus.Status == true)
                return BadRequest(errorStatus.Name);
            return Ok(sentRequests);
        }

        [HttpPost("AcceptOrDeclineRequest/{choise}")]
        public async Task<IActionResult> AcceptOrDeclineRequest([FromRoute] int choise, Request request)
        {
            var (errorStatus, acceptedRequests) = await requestService.AcceptOrDeclineRequest(choise, request, httpContextAccessor.HttpContext);
            if (errorStatus.Status == true)
                return BadRequest(errorStatus.Name);
            return Ok(acceptedRequests);
        }

        [HttpGet("GetRecivedRequests")]
        public async Task<IActionResult> GetRecivedRequests()
        {
            var (errorStatus, recivedRequests) = await requestService.GetRecivedRequests(httpContextAccessor.HttpContext);
            if (errorStatus.Status == true)
                return BadRequest(errorStatus.Name);
            return Ok(recivedRequests);
        }

    }
}
