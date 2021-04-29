using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using NeighborHelp.Models;
using NeighborHelp.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NeighborHelp.Controllers
{
    [Route("/[Controller]/[Action]")]
    public class AuthentificationController:Controller
    {
        private IUserDirectoryServise _userDirectory;

        public AuthentificationController(IUserDirectoryServise service)
        {
            _userDirectory = service;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return Content("Please, use Login.html page or use Post method Login(string login, string password) manyally");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string login, string password)
        {
            var user = _userDirectory.GetUser(login, password);

            bool authorizationResult = user != null;

            if (authorizationResult)
            {
                await Authenticate(user);

                return new OkResult();
            }
            else
            {
                return new NotFoundResult();
            }
        }

        private async Task Authenticate(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Id.ToString()),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name)
            };

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}
