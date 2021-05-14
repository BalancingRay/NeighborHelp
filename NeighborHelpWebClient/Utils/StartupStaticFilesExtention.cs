using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using NeighborHelpInfrastucture.Utils;

namespace NeighborHelpWebClient.Utils
{
    public static class StartupStaticFilesExtention
    {
        public const string UseStaticFilesPropertyName = "UseStaticFiles";

        public static IApplicationBuilder UseWebClientStaticFiles(this IApplicationBuilder app, IConfiguration DirectoryConfiguration, IWebHostEnvironment env)
        {
            bool useStatic = DirectoryConfiguration.ReadBoolProperty(UseStaticFilesPropertyName);

            if (useStatic)
            {
                var path = env.WebRootPath;

                if (DirectoryConfiguration["ASPNETCORE_ENVIRONMENT"] == "Development")
                {
                    path = GetRedirectedWebPath(path);
                    env.WebRootPath = path;
                }

                var options = new SharedOptions()
                {
                    FileProvider = new PhysicalFileProvider(path)
                };
                var fileOptions = new DefaultFilesOptions(options);
                fileOptions.DefaultFileNames.Add("Login.html");
                fileOptions.DefaultFileNames.Add("Chat.html");

                app.UseDefaultFiles(fileOptions);
                app.UseStaticFiles(new StaticFileOptions(options));
            }
            return app;
        }

        private static string GetRedirectedWebPath(string currentPath)
        {
            var newPath = currentPath.Replace("\\NeighborHelp\\NeighborHelp\\wwwroot", "\\NeighborHelp\\NeighborHelpWebClient\\wwwroot");
            return newPath;
        }
    }
}
