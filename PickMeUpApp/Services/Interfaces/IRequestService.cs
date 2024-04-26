using PickMeUpApp.Models.DTO;
using PickMeUpApp.Models;

namespace PickMeUpApp.Services.Interfaces
{
    public interface IRequestService
    {
        Task<(ErrorProvider, Request)> SendRequest(dtoRequestSent dtoRequest);
        Task<(ErrorProvider, List<Request>)> GetSentRequests(string passengerEmail);
        Task<(ErrorProvider, Request)> AcceptOrDeclineRequest(int choise, dtoRequestRecived request);
        Task<(ErrorProvider, List<Request>)> GetRecivedRequests(string email);
    }
}
