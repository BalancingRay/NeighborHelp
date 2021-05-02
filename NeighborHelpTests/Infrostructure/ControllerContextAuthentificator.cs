using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NeighborHelp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NeighborHelpTests.Infrostructure
{
    internal static class ControllerContextAuthentificator
    {
        internal static ControllerContext Authentificate(User user)
        {
            var context = new ControllerContext();
            context.HttpContext = new DefaultHttpContext();

            Authenticate(context.HttpContext, user);
            return context;
        }

        private static void Authenticate(HttpContext httpContext, User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Id.ToString()),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role)
            };

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            httpContext.User = new ClaimsPrincipal(id);
        }
    }
}
