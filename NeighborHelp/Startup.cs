using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NeighborHelp.Utils;
using Microsoft.Extensions.Hosting;
using NeighborHelp.Properties.Enums;
using NeighborHelp.Services;
using NeighborHelp.Services.Contracts;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using NeighborHelpAPI.Consts;

namespace NeighborHelp
{
    public class Startup
    {
        private const string ConnectionPropertyName = "DefaultConnection";
        private bool ClearDBOnStart = true;
        private bool UseInMemotyDB = false;
        private AuthentificationType authentificationType;

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            ClearDBOnStart = Configuration.ReadBoolProperty(nameof(ClearDBOnStart), ClearDBOnStart);
            UseInMemotyDB = Configuration.ReadBoolProperty(nameof(UseInMemotyDB), UseInMemotyDB);
            authentificationType = Configuration.ReadAuthentificationType();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            ConfigureAuthentification(services);
            ConfigureDirectoryServices(services);

            services.AddAuthorization();
        }

        private IServiceCollection ConfigureAuthentification(IServiceCollection services)
        {
            switch (authentificationType)
            {
                case AuthentificationType.NONE:
                    services.AddSingleton<IAuthorizationHandler, AllowAnonymous>();
                    services.TryAddSingleton<IPolicyEvaluator, DisableAuthenticationPolicyEvaluator>();
                    break;

                case AuthentificationType.COOKIES:
                    services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options =>
                    {
                        options.LoginPath = new PathString(PathConst.LOGIN_BY_COOKIES_PATH);
                        options.AccessDeniedPath = new PathString(PathConst.LOGIN_BY_COOKIES_PATH);
                    });
                    break;

                case AuthentificationType.JWT:
                    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                   .AddJwtBearer(options =>
                   {
                       options.RequireHttpsMetadata = false;
                       options.SaveToken = true;

                       options.TokenValidationParameters = new TokenValidationParameters()
                       {
                           ValidIssuer = Configuration["Tokens:Issuer"],
                           ValidAudience = Configuration["Tokens:Issuer"],
                           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"]))
                       };
                   });
                    break;
            }

            return services;
        }

        private IServiceCollection ConfigureDirectoryServices(IServiceCollection services)
        {
            if (!UseInMemotyDB)
            {
                string connection = Configuration.GetConnectionString(ConnectionPropertyName);

                //Don't use ApplicationContext directly. Use EntityUserOrderDirectory class instead.
                //services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));

                var options = new DbContextOptionsBuilder<ApplicationContext>()
                        .UseSqlServer(connection)
                        .Options;

                services.AddScoped(typeof(IOrderDirectoryServise),
                    (servProvider) =>
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

                var testData = new TestDataFiller(directoryService, directoryService);
                testData.FillIfEmpty();
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
