using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NeighborHelp.Utils;
using NeighborHelpAPI.Consts;
using NeighborHelpChat.Hubs;
using NeighborHelpWebClient.Utils;

namespace NeighborHelp
{
    public class Startup
    {
        private static string AuthenticationConfigurationArea = AuthenticationConfigurationExtention.ConfigurationArea;
        private const string DataBaseConfigurationArea = "DataBase";

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.ConfigureAuthentication(Configuration.GetSection(AuthenticationConfigurationArea));
            services.ConfigureDirectoryServices(Configuration.GetSection(DataBaseConfigurationArea));

            services.AddSignalR(hubOptions =>
            {
                hubOptions.EnableDetailedErrors = true;
                hubOptions.KeepAliveInterval = System.TimeSpan.FromMinutes(1);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseWebClientStaticFiles(Configuration, env);

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>(ChatHubConsts.Path);
            });
        }
    }
}
