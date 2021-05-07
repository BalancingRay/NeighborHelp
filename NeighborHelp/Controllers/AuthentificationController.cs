using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using NeighborHelpModels.Models;
using NeighborHelp.Services.Contracts;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using Microsoft.AspNetCore.Http;
using NeighborHelp.Utils;
using NeighborHelpModels.ControllersModel;
using NeighborHelpAPI.Consts;

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
            return Content("Please, use Login.html page or use Post method Login(string login, string password)");
        }

        [HttpPost]
        [ActionName(AuthenticationConsts.LOGIN_BY_JWT)]
        [AllowAnonymous]
        public IActionResult LoginJson([FromBody] AuthentificateData loginData)
        {
            if (string.IsNullOrEmpty(loginData?.Login) || string.IsNullOrEmpty(loginData?.Password))
            {
                return new BadRequestResult();
            }

            var user = _userDirectory.GetUser(loginData.Login, loginData.Password);

            if (user != null)
            {
                string tokenString = AuthenticateByJWT(user);

                return new OkObjectResult(new AuthentificateToken() 
                {
                    UserId = user.Id.ToString(),
                    Token = tokenString 
                });
            }
            else
            {
                return new NotFoundResult();
            }
        }

        [HttpPost]
        [ActionName(AuthenticationConsts.LOGIN_BY_COOKIES)]
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
            var claims = AuthorizationHelper.GenerateUserClaims(user);
            var id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        private string AuthenticateByJWT(User user)
        {
            var claims = AuthorizationHelper.GenerateUserClaims(user);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            string issuer = _configuration["Tokens:Issuer"];
            double expiresMinumes = 5;
            if (double.TryParse(_configuration["Tokens:ExpiresMinumes"], out double result))
            {
                expiresMinumes = result;
            }

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: issuer,
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(expiresMinumes),
                signingCredentials: creds);

            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;
        }
    }
}
