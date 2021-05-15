using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace NeighborHelp.Utils
{
    internal static class StartupControllersExtention
    {
        internal static IServiceCollection ConfigureControllers(this IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
            return services;
        }
    }
}
