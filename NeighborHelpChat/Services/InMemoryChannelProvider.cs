using Microsoft.AspNetCore.SignalR;
using NeighborHelpChat.Services.Contracts;

namespace NeighborHelpChat.Services
{
    public class InMemoryChannelProvider:IChatChannelsProvider
    {
        public InMemoryChannelProvider(IGroupManager groupManager)
        {

        }
    }
}
