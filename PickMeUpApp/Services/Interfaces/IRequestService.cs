using PickMeUpApp.Models.DTO;
using PickMeUpApp.Models;

namespace PickMeUpApp.Services.Interfaces
{
    public interface IRequestService
    {
        Task<(ErrorProvider, Request)> SendRequest(dtoRequestSent dtoRequest, HttpContext httpContext);
        Task<(ErrorProvider, List<Request>)> GetSentRequests(HttpContext httpContext);
        Task<(ErrorProvider, Request)> AcceptOrDeclineRequest(int choise, Request request, HttpContext httpContext);
        Task<(ErrorProvider, List<Request>)> GetRecivedRequests(HttpContext httpContext);
    }
}
