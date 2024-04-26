using PickMeUpApp.Models;
using PickMeUpApp.Models.DTO;

namespace PickMeUpApp.Services.Interfaces
{
    public interface IUserService
    {
        Task<(ErrorProvider, List<User>)> GetUsers();
        Task<(ErrorProvider, List<TheRoute>)> GetUserRoutes(string email);
        Task<(ErrorProvider, User)> UserRegistration(dtoUserRegistration user, HttpContext httpContextAccessor);
        Task<(ErrorProvider, User)> UserLogin(dtoUserLogin userDto, HttpContext httpContextAccessor);
    }
}
