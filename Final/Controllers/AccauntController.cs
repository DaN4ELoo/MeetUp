using Final.Models;
using Final.Data;
using Final.Properties;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Final.Controllers
{
    [ApiController]
    public class AccountController : Controller
    {
        private readonly HttpContext httpContext;
        private readonly AppDbContext dataContext;

        public AccountController(IHttpContextAccessor httpContext, AppDbContext dataContext)
        {
            this.httpContext = httpContext.HttpContext;
            this.dataContext = dataContext;
        }

        [HttpPost("/registration")]
        public async Task Regestration([FromBody] User user)
        {
            if (user == null)
            {
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;  //Не удалось десериализовать объект User
                return;
            }

            // Проверка наличия пользователя с таким логином
            if (dataContext.Users.Any(u => u.Login == user.Login))
            {
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;  //Пользователь уже существует
                return;
            }

            // Хеширование пароля
            user.Password = HashPassword(user.Password);

            // Помечаем сущность как добавленную (Add)
            dataContext.Entry(user).State = EntityState.Added;

            // Сохраняем изменения
            dataContext.SaveChanges();
        }

        [HttpPost("/login")]
        public async Task LoginAuthentication([FromBody] User userFromClient)
        {

            //User userFromClient = await httpContext.Request.ReadFromJsonAsync<User>();
            User userFromDb = dataContext.Users.FirstOrDefault(u => u.Login == userFromClient.Login);
            if (userFromDb == null)
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }
            if (!(VerifyPassword(userFromClient.Password, userFromDb.Password)))
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, userFromClient.Login) };
            JwtSecurityToken jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)), // время действия 2 минуты
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            await httpContext.Response.WriteAsync(new JwtSecurityTokenHandler().WriteToken(jwt));
        }

        private string HashPassword(string password)
        {
            // Хеширование пароля без использования соли
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: new byte[0], // Пустая соль
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return hashed;
        }

        private bool VerifyPassword(string enteredPassword, string hashedPassword)
        {
            // Проверка введенного пароля с использованием хеша из базы данных
            var actualHash = KeyDerivation.Pbkdf2(
                password: enteredPassword,
                salt: new byte[0], // Пустая соль
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);

            // Сравнение хешей
            return actualHash.SequenceEqual(Convert.FromBase64String(hashedPassword));
        }
    }
}
