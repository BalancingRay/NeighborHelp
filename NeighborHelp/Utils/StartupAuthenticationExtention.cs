using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using NeighborHelp.Properties.Enums;
using NeighborHelp.Services;
using NeighborHelpAPI.Consts;
using System.Text;
using NeighborHelpInfrastucture.Utils;

namespace NeighborHelp.Utils
{
    internal static class StartupAuthenticationExtention
    {
        //Exception happents if following multuScheme add to Controllers:
        public static readonly string MultipleaAthSchemes =
        CookieAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme;

        //TODO First solution : add mocking of cookies or jwt scheme if it not in parameters but requared by controller Attribute
        //TODO Second solution: add custom Authentication handler to skip exception if current parameter not supporting scheme

        internal static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration authenticationConfiguration)
        {
            //var authentificationType = authenticationConfiguration.ReadAuthentificationType();
            var authentificationType = AuthenticationConfigurationExtention.ParseAuthenticationType(AuthorizeAttributeHelper.CurrentSetting);
            //Temp solution. Ignor AppSettings and use constant from AuthorizeAttributeHelper
            //to generate value for AuthorizeAttribute.AuthenticationSchemes for controllers actions

            switch (authentificationType)
            {
                case AuthentificationType.NONE:
                    services.AddNoneAuthentication();
                    break;

                case AuthentificationType.COOKIES:
                    services.AddCookiesAuthentication();
                    break;

                case AuthentificationType.JWT:
                    services.AddJWTAuthentication(authenticationConfiguration);
                    break;
                case AuthentificationType.COOKIES_AND_JWT:
                    services.AddJWTAuthentication(authenticationConfiguration);
                    services.AddCookiesAuthentication();
                    break;
            }
            return services;
        }

        private static void AddNoneAuthentication(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationHandler, AllowAnonymous>();
            services.TryAddSingleton<IPolicyEvaluator, DisableAuthenticationPolicyEvaluator>();
        }

        private static void AddCookiesAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString(PathConst.LOGIN_BY_COOKIES_PATH);
                    options.AccessDeniedPath = new PathString(PathConst.LOGIN_BY_COOKIES_PATH);
                });
        }

        private static void AddJWTAuthentication(this IServiceCollection services, IConfiguration authenticationConfiguration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;

                    string issuer = authenticationConfiguration.GetTokenIssuer();
                    string token = authenticationConfiguration.GetTokenKey();
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = issuer,
                        ValidAudience = issuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(token))
                    };
                });
        }
    }
}
