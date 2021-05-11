using NeighborHelpModels.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace NeighborHelp.Utils
{
    public static class AuthorizationHelper
    {
        internal static IEnumerable<Claim> GenerateUserClaims(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Id.ToString()),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role)
            };

            return claims;
        }

        internal static bool TryGetCurrentUserId(ClaimsPrincipal user, out int userId)
        {
            string claimId = user?.Claims?.FirstOrDefault(cl => cl.Type == ClaimsIdentity.DefaultNameClaimType)?.Value;

            if (int.TryParse(claimId, out int id))
            {
                userId = id;
                return true;
            }
            else
            {
                userId = 0;
                return false;
            }
        }

        internal static string GetCurrentUserRole(ClaimsPrincipal user)
        {
            string role = user?.Claims?.FirstOrDefault(cl => cl.Type == ClaimsIdentity.DefaultRoleClaimType)?.Value;
            return role;
        }
    }
}
