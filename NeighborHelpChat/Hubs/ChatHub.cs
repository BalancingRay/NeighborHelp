using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using NeighborHelpAPI.Consts;
using NeighborHelpChat.Services;
using NeighborHelpChat.Services.Contracts;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using NeighborHelpInfrastucture.Utils;

namespace NeighborHelpChat.Hubs
{
    [Authorize(AuthenticationSchemes = AuthorizeAttributeHelper.Value)]
    public class ChatHub : Hub
    {
        private IChatChannelsProvider channels;
        private IChatUserProvider users;

        public ChatHub(/*IChatUserProvider userProvider*/)
        { 
            channels = new InMemoryChannelProvider(Groups);
            //users = userProvider;
        }

        [HubMethodName(ChatHubConsts.EnderToGroup)]
        public async Task Enter(string groupname)
        {
            if (!string.IsNullOrEmpty(groupname))
            {
                string username = GetUserName();

                await Groups.AddToGroupAsync(Context.ConnectionId, groupname);
                await Clients.Group(groupname).SendAsync(ChatHubConsts.NotifyClients, $"{username} вошел в чат");
            }
        }

        [HubMethodName(ChatHubConsts.SendMessage)]
        public async Task Send(string message)
        {
            await Clients.All.SendAsync(ChatHubConsts.ReceiveClientsMesage, message, GetUserName());
        }

        [HubMethodName(ChatHubConsts.SendToGroup)]
        public async Task SendToGroup(string message, string group)
        {
            await Clients.Group(group).SendAsync(ChatHubConsts.ReceiveClientsMesage, message, GetUserName());
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("Notify", $"{GetUserName()} вошел в чат");
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.All.SendAsync("Notify", $"{GetUserName()} покинул в чат");
            await base.OnDisconnectedAsync(exception);
        }

        private string GetUserName()
        {
            if (TryGetCurrentUserId(Context?.User, out int userId))
            {
                //TODO get userName from IUserDirectoryService
                return userId.ToString();
            }
            else
            {
                string userName = Context.ConnectionId;
                int maxLength = 10;
                int length = userName.Length;
                if (length > maxLength)
                {
                    userName = userName.Substring(length - maxLength);
                    userName = $"anonim_{userName}";
                }

                return userName;
            }
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
    }
}
