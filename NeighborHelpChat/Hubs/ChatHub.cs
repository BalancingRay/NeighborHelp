using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using NeighborHelpAPI.Consts;
using NeighborHelpChat.Services;
using NeighborHelpChat.Services.Contracts;
using NeighborHelpInfrastucture.Utils;
using NeighborHelpInfrastructure.ServiceContracts;
using System;
using System.Threading.Tasks;


namespace NeighborHelpChat.Hubs
{
    [Authorize(AuthenticationSchemes = AuthorizeAttributeHelper.Value)]
    public class ChatHub : Hub
    {
        private IChatUserProvider users;

        public string CurrentUserName => users.GetCurrentUserName(Context);

        public ChatHub(IUserDirectoryServise userService)
        {
            users = new ChatUserProvider(userService);
        }

        [HubMethodName(ChatHubConsts.EnderToGroup)]
        public async Task Enter(string groupname)
        {
            if (!string.IsNullOrEmpty(groupname))
            {
                string username = CurrentUserName;

                await Groups.AddToGroupAsync(Context.ConnectionId, groupname);
                await Clients.Group(groupname).SendAsync(ChatHubConsts.NotifyClients, $"{username} joined to the group chat");
            }
        }

        [HubMethodName(ChatHubConsts.SendMessage)]
        public async Task Send(string message)
        {
            string username = users.GetCurrentUserName(Context);
            await Clients.All.SendAsync(ChatHubConsts.ReceiveClientsMesage, message, CurrentUserName);
        }

        [HubMethodName(ChatHubConsts.SendToGroup)]
        public async Task SendToGroup(string message, string group)
        {
            await Clients.Group(group).SendAsync(ChatHubConsts.ReceiveClientsMesage, message, CurrentUserName);
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("Notify", $"{CurrentUserName} joined to the chat");
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.All.SendAsync("Notify", $"{CurrentUserName} left the chat");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
