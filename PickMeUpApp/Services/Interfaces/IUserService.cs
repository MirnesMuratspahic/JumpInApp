using PickMeUpApp.Models;
using PickMeUpApp.Models.DTO;

namespace PickMeUpApp.Services.Interfaces
{
    public interface IUserService
    {
        Task<(ErrorProvider, string)> UserRegistration(dtoUserRegistration user);
        Task<(ErrorProvider, string)> UserLogin(dtoUserLogin userDto);
        Task<(ErrorProvider, dtoTheRoute)> AddRoute(dtoUserRoute route);
    }
}
