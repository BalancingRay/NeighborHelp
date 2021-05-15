using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using NeighborHelpAPI.Consts;
using NeighborHelpChat.Hubs;
using System;

namespace NeighborHelpChat.Utils
{
    public static class StartupChatExtention
    {
        private static int connectionInterval = 5;
        public static IServiceCollection ConfigureChatHub(this IServiceCollection services)
        {
            services.AddSignalR(hubOptions =>
            {
                hubOptions.EnableDetailedErrors = true;
                hubOptions.KeepAliveInterval = TimeSpan.FromMinutes(connectionInterval);
            });
            return services;
        }

        public static HubEndpointConventionBuilder MapChatHub(this IEndpointRouteBuilder endpoints)
        {
            return endpoints.MapHub<ChatHub>(ChatHubConsts.Path);
        }
    }
}
