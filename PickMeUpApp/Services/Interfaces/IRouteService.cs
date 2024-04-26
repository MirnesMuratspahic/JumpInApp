using PickMeUpApp.Models.DTO;
using PickMeUpApp.Models;

namespace PickMeUpApp.Services.Interfaces
{
    public interface IRouteService
    {
        Task<(ErrorProvider, List<UserRoute>)> GetRoutes();
        Task<(ErrorProvider, TheRoute)> AddRoute(dtoUserRoute route);
    }
}
