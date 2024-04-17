using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PickMeUpApp.Context;
using PickMeUpApp.Models;
using PickMeUpApp.Models.DTO;
using PickMeUpApp.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public async Task<(ErrorProvider, string)> UserLogin(dtoUserLogin userDto)
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
            return (error, token);

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
        
    }
}

