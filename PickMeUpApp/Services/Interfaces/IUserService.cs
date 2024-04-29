using PickMeUpApp.Models;
using PickMeUpApp.Models.DTO;

namespace PickMeUpApp.Services.Interfaces
{
    public interface IUserService
    {
        Task<(ErrorProvider, List<User>)> GetUsers();
        Task<(ErrorProvider, List<TheRoute>)> GetUserRoutes(HttpContext httpContext);
        Task<(ErrorProvider, dtoUser)> UserRegistration(dtoUserRegistration user, HttpContext httpContext);
        Task<(ErrorProvider, dtoUser)> UserLogin(dtoUserLogin userDto, HttpContext httpContext);
    }
}
