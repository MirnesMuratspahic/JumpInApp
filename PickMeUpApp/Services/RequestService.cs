using Microsoft.EntityFrameworkCore;
using PickMeUpApp.Models.DTO;
using PickMeUpApp.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using PickMeUpApp.Context;
using PickMeUpApp.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using Azure.Core;
using Request = PickMeUpApp.Models.Request;

namespace PickMeUpApp.Services
{
    public class RequestService : IRequestService
    {
        public ApplicationDbContext DbContext { get; set; }
        IConfiguration configuration;
        public ErrorProvider error = new ErrorProvider() { Status = false };
        public ErrorProvider defaultError = new ErrorProvider() { Status = true, Name = "Property koji ste poslali ne smije biti null!" };
        public string EmailClaim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";


        public RequestService(ApplicationDbContext context, IConfiguration _configuration)
        {
            DbContext = context;
            configuration = _configuration;
        }

        public async Task<(ErrorProvider, Request)> SendRequest(dtoRequestSent dtoRequest, HttpContext httpContext)
        {
            if (dtoRequest == null)
                return (defaultError, null);

            var emailClaim = httpContext.User.Claims.FirstOrDefault(claim => claim.Type == EmailClaim)?.Value;
            var routeFromDatabase = await DoesExistRoute(dtoRequest.UserRoute);
            var passenger = await DbContext.Users.FirstOrDefaultAsync(x => x.Email == emailClaim);

            if (dtoRequest.UserRoute.User.Email == emailClaim)
            {
                error = new ErrorProvider()
                {
                    Status = true,
                    Name = "Ne mozete slati zahtjev za rutu koju ste vi kreirali!"
                };
                return (error, null);
            }
            if (routeFromDatabase == null)
            {
                error = new ErrorProvider()
                {
                    Status = true,
                    Name = "Ne postoji poslana ruta u bazi!"
                };
                return (error, null);
            }

            if (routeFromDatabase.Route.SeatsNumber == 0)
            {
                error = new ErrorProvider()
                {
                    Status = true,
                    Name = "Sva mjesta su popunjena!"
                };
            }

            if (passenger == null)
            {
                error = new ErrorProvider()
                {
                    Status = true,
                    Name = "Ne postoji user sa poslanom passenger adresom u bazi!"
                };
                return (error, null);
            }

            var request = new Request()
            {
                UserRoute = routeFromDatabase,
                PassengerEmail = emailClaim,
                Description = dtoRequest.Description,
                Status = dtoRequest.Status
            };

            var requestFromDatabase = await DoesExistRequest(request);

            if(requestFromDatabase != null && requestFromDatabase.Status.ToLower() == "pending") 
            {
                error = new ErrorProvider()
                {
                    Status = true,
                    Name = "Vec ste poslali zahtjev za tu rutu!"
                };
                return (error, null);
            }

            await DbContext.Requests.AddAsync(request);
            await DbContext.SaveChangesAsync();

            return (error, request);
        }

        private async Task<Request> DoesExistRequest(Request request)
        {
            if (await DoesExistRoute(request.UserRoute) != null)
            {
                var requestfromDatabase = await DbContext.Requests.Where(x => x.PassengerEmail == request.PassengerEmail &&
                                                                          x.Status.ToLower() == request.Status.ToLower()).Include(x => x.UserRoute.User)
                                                                         .Include(x => x.UserRoute.Route).FirstOrDefaultAsync();
                return requestfromDatabase;
            }
            return null;
        }
        private async Task<UserRoute> DoesExistRoute(UserRoute route)
        { 
            var routeFromDatabase = await DbContext.UserRoutes.Include(x=>x.User).Include(x=>x.Route).Where(x => x.User.Email == route.User.Email &&
                                                       x.Route.Name == route.Route.Name &&
                                                       x.Route.SeatsNumber == route.Route.SeatsNumber &&
                                                       x.Route.DateAndTime == route.Route.DateAndTime &&
                                                       x.Route.Description == route.Route.Description).FirstOrDefaultAsync();
            return (routeFromDatabase);
        }


        public async Task<(ErrorProvider, List<Request>)> GetSentRequests(HttpContext httpContext)
        {

            var emailClaim = httpContext.User.Claims.FirstOrDefault(claim => claim.Type == EmailClaim)?.Value;

            var passenger = await DbContext.Users.FirstOrDefaultAsync(x => x.Email == emailClaim);

            if (passenger == null)
            {
                error = new ErrorProvider()
                {
                    Status = true,
                    Name = "Ne postoji user sa poslanom email adresom!"
                };
                return (error, null);
            }

            var dtoRequests = await DbContext.Requests.Where(x => x.PassengerEmail == emailClaim)
                .Include(x => x.UserRoute.User).Include(x => x.UserRoute.Route).ToListAsync();


            if (dtoRequests.Count == 0)
            {
                error = new ErrorProvider()
                {
                    Status = true,
                    Name = "Ne postoji niti jedan request!"
                };
                return (error, null);
            }

            return (error, dtoRequests);
        }

        public async Task<(ErrorProvider, Request)> AcceptOrDeclineRequest(int choise, Request request, HttpContext httpContext)
        {
            if (request == null || choise == null)
                return (defaultError, null);

            var emailClaim = httpContext.User.Claims.FirstOrDefault(claim => claim.Type == EmailClaim)?.Value;
            if (choise != 0 && choise != 1)
            {
                error = new ErrorProvider()
                {
                    Status = true,
                    Name = "Polje choise nije validno!"
                };
                return (error, null);
            }

            var requestFromDatabase = await DoesExistRequest(request);

            if (requestFromDatabase == null)
            {
                error = new ErrorProvider()
                {
                    Status = true,
                    Name = "Ne postoji taj request u bazi!"
                };
                return (error, null);
            }

            if (choise == 0)
            {
                requestFromDatabase.Status = "Declined";
            }
            else if (choise == 1)
            {
                requestFromDatabase.Status = "Accepted";
                requestFromDatabase.UserRoute.Route.SeatsNumber -= 1;
            }

            await DbContext.SaveChangesAsync();
            request.Status = "Accepted";

            return (error, requestFromDatabase);

        }

        public async Task<(ErrorProvider, List<Request>)> GetRecivedRequests(HttpContext httpContext)
        {
            var emailClaim = httpContext.User.Claims.FirstOrDefault(claim => claim.Type == EmailClaim)?.Value;
            var userFromDatabase = await DbContext.Users.FirstOrDefaultAsync(x => x.Email == emailClaim);

            if (userFromDatabase == null)
            {
                error = new ErrorProvider()
                {
                    Status = true,
                    Name = "Ne postoji user sa poslanom email adresom!"
                };
                return (error, null);
            }

            var requests = await DbContext.Requests.Where(x => x.UserRoute.User.Email == emailClaim && x.Status.ToLower() == "pending")
                .Include(x => x.UserRoute.User).Include(x => x.UserRoute.Route).ToListAsync();

            if (requests.Count == 0)
            {
                error = new ErrorProvider()
                {
                    Status = true,
                    Name = "User nema dobijenih requestova!"
                };
                return (error, null);
            }

            return (error, requests);
        }
    }
}
