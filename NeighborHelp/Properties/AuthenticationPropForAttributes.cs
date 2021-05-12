using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace NeighborHelp.Properties
{
    public static class AuthenticationPropForAttributes
    {
        private const string Cookies_and_JWT = "COOKIES_AND_JWT";
        private const string Cookies = "COOKIES";
        private const string JWT = "JWT";
        private const string None = "NONE";

        //Change this value to select necessary scheme
        public const string CurrentSetting = Cookies_and_JWT;

        private const string CookiesScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        private const string JWTScheme = JwtBearerDefaults.AuthenticationScheme;

        public const string Value = (CurrentSetting == Cookies) ? CookiesScheme
            : (CurrentSetting == JWT) ? JWTScheme
            : (CurrentSetting == Cookies_and_JWT) ? CookiesScheme +", "+ JWTScheme
            : "";
    }
}
