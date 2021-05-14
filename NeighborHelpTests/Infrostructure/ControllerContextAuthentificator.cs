using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NeighborHelpModels.Models;
using System.Collections.Generic;
using System.Security.Claims;

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
