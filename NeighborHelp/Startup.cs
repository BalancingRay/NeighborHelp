using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NeighborHelp.Services;
using NeighborHelp.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeighborHelp
{
    public class Startup
    {
        private bool ClearDBOnStart = true;
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

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/Authentification/Login");
                    options.AccessDeniedPath = new PathString("/Authentification/Login");
                });
            services.AddAuthorization();

            ConfigureDirectory(services);
        }

        private IServiceCollection ConfigureDirectory(IServiceCollection services)
        {

            //Use MS SQL server implementation
            string connection = Configuration.GetConnectionString("DefaultConnection");

            var options = new DbContextOptionsBuilder<ApplicationContext>()
                    .UseSqlServer(connection)
                    .Options;

            ApplicationContext applicationContext = new ApplicationContext(options, ClearDBOnStart);
            var directoryService = new EntityUserOrderDirectory(applicationContext);

            //Use memory implementation
            //var directoryService = new MemoryUserOrderDirectory();

            var testData = new TestDataFiller(directoryService, directoryService);
            testData.FillIfEmpty();

            services.AddSingleton(typeof(IOrderDirectoryServise), directoryService);
            services.AddSingleton(typeof(IUserDirectoryServise), directoryService);

            return services;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
