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
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Net.Http;

namespace PickMeUpApp.Services

{
    public class UserService : IUserService
    {
        public ApplicationDbContext DbContext { get; set; }
        public IConfiguration configuration { get; set; }
        public ErrorProvider error = new ErrorProvider() { Status = false};
        public ErrorProvider defaultError = new ErrorProvider() { Status = true, Name = "Property koji ste poslali ne smije biti null!" };
        public string EmailClaim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";

        public UserService() { }
        public UserService(ApplicationDbContext context, IConfiguration _configuration)
        {
            DbContext = context;
            configuration = _configuration;
        }


        public async Task<(ErrorProvider, List<User>)> GetUsers()
        {
            var users = await DbContext.Users.ToListAsync();
            if (users.Count == 0)
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

        public async Task<(ErrorProvider, User)> GetUserByEmail(string email)
        {
            if(email == null)
            {
                return(defaultError, null);
            }

            var userFromDatabase = await DbContext.Users.FirstOrDefaultAsync(x => x.Email == email);

            if (userFromDatabase == null)
            {
                error = new ErrorProvider()
                {
                    Status = true,
                    Name = "Ne postoji user sa poslanim emailom!"

                };
                return (error, null);
            }

            return (error, userFromDatabase);
        }

        public async Task<(ErrorProvider, List<TheRoute>)> GetUserRoutes(HttpContext httpContext)
        {

            var emailClaim = httpContext.User.Claims.FirstOrDefault(claim => claim.Type == EmailClaim)?.Value;

            var user = await DbContext.Users.FirstOrDefaultAsync(x => x.Email == emailClaim);

            if (user == null)
            {
                error = new ErrorProvider()
                {
                    Status = true,
                    Name = "U bazi se ne nalazi user sa datom email adresom!"
                };
                return (error, null);
            }

            var routesIds = await DbContext.UserRoutes.Where(x => x.UserId == user.Id).Select(x => x.RouteId).ToListAsync();
            var routes = await DbContext.Routes.Where(x => routesIds.Contains(x.Id)).ToListAsync();

            if (routes.Count == 0)
            {
                error = new ErrorProvider()
                {
                    Status = true,
                    Name = $"User {user.FirstName} {user.LastName} nema niti jednu dodanu rutu!"
                };
                return (error, null);
            }
            return (error, routes);
        }
    

    public async Task<(ErrorProvider, dtoUser)> UserRegistration(dtoUserRegistration userDto, HttpContext httpContext)
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

            var newDtoUser = new dtoUser()
            {
                UserToken = token,
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Email = newUser.Email,
                PhoneNumber = newUser.PhoneNumber
            };

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.Now.AddDays(1),
                Secure = true,
                SameSite = SameSiteMode.Lax
            };

            httpContext.Response.Cookies.Append("jwtToken", token, cookieOptions);

            return (error, newDtoUser);

        }

        public async Task<(ErrorProvider, dtoUser)> UserLogin(dtoUserLogin userDto, HttpContext httpContext)
        {
            if (userDto == null)
                return (defaultError, null);

            var userFromDatabase = await DbContext.Users.FirstOrDefaultAsync(x => x.Email == userDto.Email);

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

            var newDtoUser = new dtoUser()
            {
                UserToken = token,
                FirstName = userFromDatabase.FirstName,
                LastName = userFromDatabase.LastName,
                Email = userFromDatabase.Email,
                PhoneNumber = userFromDatabase.PhoneNumber
            };
            var cookieOptions = new CookieOptions
            {
                HttpOnly = false,
                Expires = DateTime.Now.AddDays(1),
                Secure = true,
                SameSite = SameSiteMode.Lax
            };

            httpContext.Response.Cookies.Append("jwtToken", token, cookieOptions);


            return (error, newDtoUser);

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



    }
}


