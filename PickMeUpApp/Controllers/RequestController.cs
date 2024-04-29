using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PickMeUpApp.Models;
using PickMeUpApp.Models.DTO;
using PickMeUpApp.Services.Interfaces;

namespace PickMeUpApp.Controllers
{
    public class RequestController : BaseController
    {
        public IRequestService _requestService { get; set; }
        public RequestController(IRequestService requestService, IHttpContextAccessor _httpContextAccessor) : base(_httpContextAccessor)
        {
            _requestService = requestService;
        }

        [HttpPost("SendRequest")]
        public async Task<IActionResult> SendRequest(dtoRequestSent dtoRequest)
        {
            var (errorStatus, request) = await _requestService.SendRequest(dtoRequest, httpContextAccessor.HttpContext);
            if (errorStatus.Status)
                return BadRequest(errorStatus.Name);
            return Ok(request);
        }

        [HttpGet("GetSentRequests")]
        public async Task<IActionResult> GetSentRequests()
        {
            var (errorStatus, sentRequests) = await _requestService.GetSentRequests(httpContextAccessor.HttpContext);
            if (errorStatus.Status)
                return BadRequest(errorStatus.Name);
            return Ok(sentRequests);
        }

        [HttpPost("AcceptOrDeclineRequest/{choise}")]
        public async Task<IActionResult> AcceptOrDeclineRequest([FromRoute] int choise, Request request)
        {
            var (errorStatus, acceptedRequests) = await _requestService.AcceptOrDeclineRequest(choise, request, httpContextAccessor.HttpContext);
            if (errorStatus.Status)
                return BadRequest(errorStatus.Name);
            return Ok(acceptedRequests);
        }

        [HttpGet("GetRecivedRequests")]
        public async Task<IActionResult> GetRecivedRequests()
        {
            var (errorStatus, recivedRequests) = await _requestService.GetRecivedRequests(httpContextAccessor.HttpContext);
            if (errorStatus.Status)
                return BadRequest(errorStatus.Name);
            return Ok(recivedRequests);
        }

    }
}
