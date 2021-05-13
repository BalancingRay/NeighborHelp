using Microsoft.AspNetCore.SignalR;
using NeighborHelpChat.Services.Contracts;
using NeighborHelpInfrastructure.ServiceContracts;
using NeighborHelpInfrastucture.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeighborHelpChat.Services
{
    public class ChatUserProvider : IChatUserProvider
    {
        private const string anonimPrefix = "anonim_";
        private const string undefinedUserPrefix = "undefined_";
        private const int maxKeyLength = 8;

        IUserDirectoryServise users;
        public ChatUserProvider(IUserDirectoryServise userService)
        {
            users = userService;
        }

        public string GetCurrentUserName(HubCallerContext Context)
        {
            if (AuthorizationHelper.TryGetCurrentUserId(Context?.User, out int userId))
            {
                string userName = users.GetUser(userId)?.UserName;

                if (string.IsNullOrWhiteSpace(userName))
                {
                    return BuildNameByPrefixAndKey(undefinedUserPrefix, Context.ConnectionId);
                }

                return userName.ToString();
            }
            else
            {
                return BuildNameByPrefixAndKey(anonimPrefix, Context?.ConnectionId);
            }
        }

        private string BuildNameByPrefixAndKey(string prefix, string key)
        {
            if (key.Length > maxKeyLength)
            {
                key = key.Substring(0, maxKeyLength);
            }
            return $"{prefix}{key}";
        }

        //internal static bool TryGetCurrentUserId(ClaimsPrincipal user, out int userId)
        //{
        //    string claimId = user?.Claims?.FirstOrDefault(cl => cl.Type == ClaimsIdentity.DefaultNameClaimType)?.Value;

        //    if (int.TryParse(claimId, out int id))
        //    {
        //        userId = id;
        //        return true;
        //    }
        //    else
        //    {
        //        userId = 0;
        //        return false;
        //    }
        //}
    }
}
