using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using NeighborHelpInfrastucture.Utils;
using System.IO;
using System.Reflection;

namespace NeighborHelpWebClient.Utils
{
    public static class StartupStaticFilesExtention
    {
        public const string UseStaticFilesPropertyName = "UseStaticFiles";
        private const string WebRootFolderName = "wwwroot";

        public static IApplicationBuilder UseWebClientStaticFiles(this IApplicationBuilder app, IConfiguration DirectoryConfiguration, IWebHostEnvironment env)
        {
            bool useStatic = DirectoryConfiguration.ReadBoolProperty(UseStaticFilesPropertyName);

            if (useStatic)
            {
                var path = env.WebRootPath;

                if (env.IsDevelopment() || string.IsNullOrWhiteSpace(path))
                {
                    path = BuildWebRootPath();
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

        private static string BuildWebRootPath()
        {
            string currentAssemblyPath = Assembly.GetAssembly(typeof(StartupStaticFilesExtention)).Location;
            string theDirectory = Path.GetDirectoryName(currentAssemblyPath);
            string fullPath = Path.Combine(theDirectory, WebRootFolderName);

            return fullPath;
        }
    }
}
