using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NeighborHelp.Controllers.Consts;
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
        private const string ConnectionPropertyName = "DefaultConnection";
        private bool ClearDBOnStart = true;
        private bool UseSQLDatabase = true;
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
                    options.LoginPath = new PathString(PathConst.LOGIN_PATH);
                    options.AccessDeniedPath = new PathString(PathConst.LOGIN_PATH);
                });
            services.AddAuthorization();

            ConfigureDirectory(services);
        }

        private IServiceCollection ConfigureDirectory(IServiceCollection services)
        {
            if (UseSQLDatabase)
            {
                string connection = Configuration.GetConnectionString(ConnectionPropertyName);

                //Don't use ApplicationContext directly. Use EntityUserOrderDirectory class instead.
                //services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));

                var options = new DbContextOptionsBuilder<ApplicationContext>()
                        .UseSqlServer(connection)
                        .Options;

                services.AddScoped(typeof(IOrderDirectoryServise), 
                    (servProvider)=> 
                    new EntityUserOrderDirectory(new ApplicationContext(options)));
                services.AddScoped(typeof(IUserDirectoryServise), 
                    (servProvider) => 
                    new EntityUserOrderDirectory(new ApplicationContext(options)));

                //Fill start data
                ApplicationContext applicationContext = new ApplicationContext(options, ClearDBOnStart);
                var directoryService = new EntityUserOrderDirectory(applicationContext);
                var testData = new TestDataFiller(directoryService, directoryService);
                testData.FillIfEmpty();         
            }
            else
            {
                //Use implementation in RAM
                var directoryService = new MemoryUserOrderDirectory();

                services.AddSingleton(typeof(IOrderDirectoryServise), directoryService);
                services.AddSingleton(typeof(IUserDirectoryServise), directoryService);
            }

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
