using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PickMeUpApp.Context;
using PickMeUpApp.Models;
using PickMeUpApp.Models.DTO;
using PickMeUpApp.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Components.Forms;

namespace PickMeUpApp.Services

{
    public class UserService : IUserService
    {
        public ApplicationDbContext DbContext { get; set; }
        public IConfiguration configuration { get; set; }
        public ErrorProvider error = new ErrorProvider() { Status = false};

        public ErrorProvider defaultError = new ErrorProvider() { Status = true, Name = "Property koji ste poslali ne smije biti null!" };

        public UserService(ApplicationDbContext context, IConfiguration _configuration)
        {
            DbContext = context;
            configuration = _configuration;
        }

        public async Task<(ErrorProvider, List<User>)> GetUsers()
        {
            var users = await DbContext.Users.ToListAsync();
            if(users.Count == 0)
            {
                error = new ErrorProvider()
                {
                    Status = true,
                    Name = "Nema usera u bazi!"
                };
                return (error, null);
            }

            return (error, users);
        }

        public async Task<(ErrorProvider, List<dtoUserRoute>)> GetRoutes()
        {
            var routes = await DbContext.UserRoutes.Select(x => new dtoUserRoute(x.User, x.Route)).ToListAsync();

            if(routes.Count == 0)
            {
                error = new ErrorProvider()
                {
                    Status = true,
                    Name = "Nema ruta u bazi!"
                };
                return (error, null);
            }
            return (error, routes);

        }

        public async Task<(ErrorProvider, List<dtoTheRoute>)> GetUserRoutes(string email)
        {
            if(email == null)
                return (defaultError, null);

            var user = await DbContext.Users.FirstOrDefaultAsync(x=>x.Email == email);

            if(user == null)
            {
                error = new ErrorProvider()
                {
                    Status = true,
                    Name = "U bazi se ne nalazi user sa datom email adresom!"
                };
                return (error, null);
            }

            var routesIds = await DbContext.UserRoutes.Where(x => x.UserId == user.UserId).Select(x => x.RouteId).ToListAsync();
            var dtoRoutes = await DbContext.Routes.Where(x => routesIds.Contains(x.Id))
                .Select(x => new dtoTheRoute() { Name = x.Name, Description = x.Description }).ToListAsync();

            if(dtoRoutes.Count == 0)
            {
                error = new ErrorProvider()
                {
                    Status = true,
                    Name = $"User {user.FirstName} {user.LastName} nema niti jednu dodanu rutu!"
                };
                return (error, null);
            }
            
            return (error, dtoRoutes);
            
        }


        public async Task<(ErrorProvider, string)> UserRegistration(dtoUserRegistration userDto)
        {
            if (userDto == null)
                return (defaultError, null);

            var user = await DbContext.Users.FirstOrDefaultAsync(x => x.Email == userDto.Email);

            if (user != null)
            {
                error = new ErrorProvider()
                {
                    Status = true,
                    Name = "Vec postoji user sa istom email adresom!"
                };
                return (error, null);
            }

            string passwordHash = BCrypt.Net.BCrypt.HashString(userDto.Password);

            var newUser = new User()
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                PasswordHash = passwordHash,
                PhoneNumber = userDto.PhoneNumber
            };

            await DbContext.Users.AddAsync(newUser);
            await DbContext.SaveChangesAsync();
            var token = CreateToken(newUser);
            return (error, token);

        }

        public async Task<(ErrorProvider, User)> UserLogin(dtoUserLogin userDto, HttpContext httpContextAccessor)
        {
            if (userDto == null)
                return (defaultError, null);

            var userFromDatabase = DbContext.Users.FirstOrDefault(x => x.Email == userDto.Email);

            if (userFromDatabase == null)
            {
                error = new ErrorProvider()
                {
                    Status = true,
                    Name = "Niste unijeli tačne podatke!"
                };
                return (error, null);
            }

            if (!BCrypt.Net.BCrypt.Verify(userDto.Password, userFromDatabase.PasswordHash))
            {
                error = new ErrorProvider()
                {
                    Status = true,
                    Name = "Niste unijeli tačne podatke!"
                };
                return (error, null);
            }
            var token = CreateToken(userFromDatabase);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.Now.AddDays(1),
                Secure = true,
                SameSite = SameSiteMode.Lax
            };

            httpContextAccessor.Response.Cookies.Append("jwtToken", token, cookieOptions);

            return (error, userFromDatabase);

        }

        private string CreateToken(User user)
        {
            List<Claim> _claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.FirstName + " " + user.LastName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
            };


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("AppSettings:Token").Value!));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
                (
                    claims: _claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: cred
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        public async Task<(ErrorProvider, dtoTheRoute)> AddRoute(dtoUserRoute route)
        {
            if (route == null)
                return (defaultError, null);

            var userFromDatabase = await DbContext.Users.FirstOrDefaultAsync(x => x.Email == route.User.Email);

            if(userFromDatabase == null)
            {
                error = new ErrorProvider()
                {
                    Status = true,
                    Name = "Ne postoji user sa poslanom email adresom!"
                };
                return (error, null);
            }

            var newRoute = new TheRoute()
            {
                Name = route.Route.Name,
                Description = route.Route.Description
            };

            await DbContext.Routes.AddAsync(newRoute);
            await DbContext.SaveChangesAsync();

            var userRoute = new UserRoute()
            {
                UserId = userFromDatabase.UserId,
                RouteId = newRoute.Id
            };

            await DbContext.UserRoutes.AddAsync(userRoute);
            await DbContext.SaveChangesAsync();

            return (error, route.Route);
        }

        public async Task<(ErrorProvider, dtoRequest)> SendRequest(dtoRequest dtoRequest)
        {
            if (dtoRequest == null)
                return (defaultError, null);

            var userId = await DbContext.Users.Where(x => x.Email == dtoRequest.dtoUserRoute.User.Email).Select(x => x.UserId).FirstOrDefaultAsync();
            var routeId = await DbContext.Routes.Where(x => x.Name == dtoRequest.dtoUserRoute.Route.Name).Select(x => x.Id).FirstOrDefaultAsync();

            var userRoute = await DbContext.UserRoutes.Include(x=>x.User).Include(x=>x.Route).FirstOrDefaultAsync(x => x.UserId == userId && x.RouteId == routeId);

            if (userRoute == null)
            {
                error = new ErrorProvider()
                {
                    Status = true,
                    Name = "Ne postoji poslana ruta u bazi!"
                };
                return (error, null);
            }
            
            var request = new Request()
            {
                UserRoute = userRoute,
                PassengerEmail = dtoRequest.passengerEmail,
                Description = dtoRequest.Description,
                Status = dtoRequest.Status
            };

            await DbContext.Requests.AddAsync(request);
            await DbContext.SaveChangesAsync();

            return (error, dtoRequest); 
        }

        public async Task<(ErrorProvider, List<dtoRequest>)> GetSentRequests(string passengerEmail)
        {
            if (string.IsNullOrEmpty(passengerEmail))
                return (defaultError, null);

            var passenger = await DbContext.Users.FirstOrDefaultAsync(x => x.Email == passengerEmail);

            if(passenger == null)
            {
                error = new ErrorProvider()
                {
                    Status = true,
                    Name = "Ne postoji user sa poslanom email adresom!"
                };
                return (error, null);
            }

            var dtoRequests = await DbContext.Requests.Where(x => x.PassengerEmail == passengerEmail).Include(x => x.UserRoute.User).Include(x=>x.UserRoute.Route)
                .Select(x => new dtoRequest(x)).ToListAsync();


            if(dtoRequests.Count == 0)
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

        public async Task<(ErrorProvider, dtoRequest)> AcceptorDeclineRequest(string choise, dtoRequest request)
        {
            if(request == null || string.IsNullOrEmpty(choise)) 
                return(defaultError, null);

            if(choise != "0" || choise != "1")
            {
                error = new ErrorProvider()
                {
                    Status = true,
                    Name = "Polje choise nije validno!"
                };
                return (error, null);   
            }    

            var requestFromDatabase = await DbContext.Requests.FirstOrDefaultAsync(x=>x.PassengerEmail == request.passengerEmail 
                                                                    && x.UserRoute.User.Email == request.dtoUserRoute.User.Email);

            if(requestFromDatabase == null)
            {
                error = new ErrorProvider()
                {
                    Status = true,
                    Name = "Ne postoji taj request u bazi!"
                };
                return (error, null);
            }

            if (choise == "0")
                requestFromDatabase.Status = "Declined";
            else if (choise == "1") 
                requestFromDatabase.Status = "Accepted";

            await DbContext.SaveChangesAsync();
            request.Status = "Accepted";

            return (error, request);
            
        }

        public async Task<(ErrorProvider, List<dtoRequest>)> GetRecivedRequests(string email)
        {
            if (string.IsNullOrEmpty(email))
                return (defaultError, null);

            var userFromDatabase = await DbContext.Users.FirstOrDefaultAsync(x => x.Email == email);

            if (userFromDatabase == null)
            {
                error = new ErrorProvider()
                {
                    Status = true,
                    Name = "Ne postoji user sa poslanom email adresom!"
                };
                return (error, null);
            }

            var requests = await DbContext.Requests.Where(x => x.UserRoute.User.Email == email).Include(x => x.UserRoute.User).Include(x => x.UserRoute.Route)
                .Select(x => new dtoRequest(x)).ToListAsync();

            if (requests.Count == 0)
            {
                error = new ErrorProvider()
                {
                    Status = true,
                    Name = "User nema dobijenih requestova!"
                };
                return (error, null);   
            }

            return (error,  requests);
        }

    }
}

