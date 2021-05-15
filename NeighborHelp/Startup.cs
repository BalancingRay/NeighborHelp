using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NeighborHelp.Utils;
using NeighborHelpChat.Utils;
using NeighborHelpWebClient.Utils;

namespace NeighborHelp
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            string authArea = AuthenticationConfigurationExtention.ConfigurationArea;
            string dbArea = StartupDataBaseExtention.ConfigurationArea;
            services.ConfigureControllers();
            services.ConfigureChatHub();
            services.ConfigureAuthentication(Configuration.GetSection(authArea));
            services.ConfigureDirectoryServices(Configuration.GetSection(dbArea));
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
                endpoints.MapChatHub();
            });
        }
    }
}
