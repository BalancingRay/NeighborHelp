using Microsoft.Extensions.Configuration;
using System;

namespace NeighborHelpInfrastucture.Utils
{
    public static class IConfigurationExtention
    {
        private readonly static string boolValue = true.ToString();

        public static void InitializeBoolProperty(this IConfiguration configuration, string propertyName, ref bool value)
        {
            string textValue = configuration.GetSection(propertyName)?.Value;

            if (!string.IsNullOrEmpty(textValue))
            {
                value = boolValue.Equals(textValue, StringComparison.OrdinalIgnoreCase);
            }
        }

        public static bool ReadBoolProperty(this IConfiguration configuration, string propertyName, bool defaultValue = false)
        {
            string textValue = configuration.GetSection(propertyName)?.Value;

            if (string.IsNullOrEmpty(textValue))
                return defaultValue;

            return boolValue.Equals(textValue, StringComparison.OrdinalIgnoreCase);
        }
    }
}
