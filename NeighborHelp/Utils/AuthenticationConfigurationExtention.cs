using Microsoft.Extensions.Configuration;
using NeighborHelp.Properties.Enums;
using System;

namespace NeighborHelp.Utils
{
    internal static class AuthenticationConfigurationExtention
    {
        public const string ConfigurationArea = "Authentication";
        private const string AuthenticationTypePropertyName = "AuthenticationType";
        private const string TokenSection = "Tokens";
        private static readonly string TokenIssuerPath = $"{TokenSection}:Issuer";
        private static readonly string TokenKeyPath = $"{TokenSection}:Key";
        private static readonly string TokenExpiresMinutesPath = $"{TokenSection}:ExpiresMinumes";

        public static AuthentificationType ReadAuthentificationType(this IConfiguration configuration)
        {
            string textValue = configuration.UseAuthenticationSection()[AuthenticationTypePropertyName];

            return ParseAuthenticationType(textValue);
        }

        public static AuthentificationType ParseAuthenticationType(string value)
        {
            var data = Enum.GetValues<AuthentificationType>();

            foreach (var item in data)
            {
                if (item.ToString().Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
            }
            return AuthentificationType.NONE;
        }

        internal static string GetTokenIssuer(this IConfiguration configuration)
        {
            return configuration.UseAuthenticationSection()[TokenIssuerPath];
        }

        internal static string GetTokenKey(this IConfiguration configuration)
        {
            return configuration.UseAuthenticationSection()[TokenKeyPath];
        }

        internal static double GetTokenTimeout(this IConfiguration configuration)
        {
            string value = configuration.UseAuthenticationSection()[TokenExpiresMinutesPath];

            if (double.TryParse(value, out double result))
            {
                return result;
            }
            else
            {
                return double.NaN;
            }
        }

        private static IConfiguration UseAuthenticationSection(this IConfiguration configuration)
        {
            var childAuthenticationSection = configuration.GetSection(ConfigurationArea);

            if (childAuthenticationSection.Exists())
            {
                return childAuthenticationSection;
            }
            else
            {
                return configuration;
            }
        }
    }
}
