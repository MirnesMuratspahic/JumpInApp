using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PickMeUpApp.Models.DTO;
using PickMeUpApp.Services.Interfaces;

namespace PickMeUpApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        public IRequestService requestService { get; set; }
        public RequestController(IRequestService _requestService) 
        {
            requestService = _requestService;
        }

        [HttpPost("SendRequest")]
        public async Task<IActionResult> SendRequest(dtoRequestSent dtoRequest)
        {
            var (errorStatus, request) = await requestService.SendRequest(dtoRequest);
            if (errorStatus.Status == true)
                return BadRequest(errorStatus.Name);
            return Ok(request);
        }

        [HttpPost("GetSentRequests")]
        public async Task<IActionResult> GetSentRequests([FromBody] string email)
        {
            var (errorStatus, sentRequests) = await requestService.GetSentRequests(email);
            if (errorStatus.Status == true)
                return BadRequest(errorStatus.Name);
            return Ok(sentRequests);
        }

        [HttpPost("AcceptOrDeclineRequest/{choise}")]
        public async Task<IActionResult> AcceptOrDeclineRequest([FromRoute] int choise, dtoRequestRecived request)
        {
            var (errorStatus, acceptedRequests) = await requestService.AcceptOrDeclineRequest(choise, request);
            if (errorStatus.Status == true)
                return BadRequest(errorStatus.Name);
            return Ok(acceptedRequests);
        }

        [HttpPost("GetRecivedRequests")]
        public async Task<IActionResult> GetRecivedRequests([FromBody] string email)
        {
            var (errorStatus, recivedRequests) = await requestService.GetRecivedRequests(email);
            if (errorStatus.Status == true)
                return BadRequest(errorStatus.Name);
            return Ok(recivedRequests);
        }

    }
}
