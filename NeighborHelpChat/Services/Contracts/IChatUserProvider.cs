using Microsoft.AspNetCore.SignalR;

namespace NeighborHelpChat.Services.Contracts
{
    public interface IChatUserProvider
    {
        string GetCurrentUserName(HubCallerContext context);
    }
}
