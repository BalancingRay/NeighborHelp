using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using NeighborHelpModels.Models;
using NeighborHelp.Services.Contracts;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;

namespace NeighborHelp.Controllers
{
    [ApiController]
    [Route("/[Controller]/[Action]")]
    public class AuthentificationController : Controller
    {
        private IUserDirectoryServise _userDirectory;

        private IConfiguration _configuration;

        public AuthentificationController(IUserDirectoryServise service, IConfiguration config)
        {
            _userDirectory = service;
            _configuration = config;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return Content("Please, use Login.html page or use Post method Login(string login, string password) manyally");
        }

        public class AuthData
        {
            public string Login { get; set; }
            public string Password { get; set; }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LoginJson([FromBody] AuthData loginData)
        {
            if (string.IsNullOrEmpty(loginData?.Login) || string.IsNullOrEmpty(loginData?.Password))
            {
                return new BadRequestResult();
            }

            var user = _userDirectory.GetUser(loginData.Login, loginData.Password);

            if (user != null)
            {
                string tokenString = AuthenticateByJWT(user);

                return new OkObjectResult(new { token = tokenString });
            }
            else
            {
                return new NotFoundResult();
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm] string login, [FromForm] string password)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                return new BadRequestResult();
            }

            var user = _userDirectory.GetUser(login, password);

            if (user != null)
            {
                await AuthenticateByCookie(user);

                return new OkResult();
            }
            else
            {
                return new NotFoundResult();
            }
        }

        private async Task AuthenticateByCookie(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Id.ToString()),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role)
            };

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        private string AuthenticateByJWT(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Id.ToString()),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role)
            };

            //var claims = new[]
            //       {
            //            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            //            new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString()),
            //        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_configuration["Tokens:Issuer"],
                _configuration["Tokens:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: creds);

            string tokenString = new JwtSecurityTokenHandler().WriteToken(token) ;
            return tokenString;
        }
    }
}
