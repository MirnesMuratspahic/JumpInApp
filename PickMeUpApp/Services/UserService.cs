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

            var userFromDatabase = await DbContext.Users.FirstOrDefaultAsync(x => x.Email == route.Email);

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

            var userId = await DbContext.Users.Where(x => x.Email == dtoRequest.dtoUserRoute.Email).Select(x => x.UserId).FirstOrDefaultAsync();
            var routeId = await DbContext.Routes.Where(x => x.Name == dtoRequest.dtoUserRoute.Route.Name).Select(x => x.Id).FirstOrDefaultAsync();

            var userRoute = await DbContext.UserRoutes.FirstOrDefaultAsync(x => x.UserId == userId && x.RouteId == routeId);

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

            var dtoRequests = await DbContext.Requests.Where(x => x.PassengerEmail == passengerEmail).Select(x => new dtoRequest()
            {
                dtoUserRoute = new dtoUserRoute() { Email = x.UserRoute.User.Email, Route = new dtoTheRoute() { Name = x.UserRoute.Route.Name, Description = x.UserRoute.Route.Description } }, 
                passengerEmail = x.PassengerEmail,
                Description = x.Description,
                Status = x.Status
            }).ToListAsync();


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
        
    }
}

