using Microsoft.Extensions.Configuration;
using NeighborHelp.Properties.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeighborHelp.Utils
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

            return  boolValue.Equals(textValue, StringComparison.OrdinalIgnoreCase);

        }

        public static AuthentificationType ReadAuthentificationType(this IConfiguration configuration)
        {
            string textValue = configuration.GetSection(nameof(AuthentificationType))?.Value;

            var data = Enum.GetValues<AuthentificationType>();

            foreach (var item in data)
            {
                if (item.ToString().Equals(textValue, StringComparison.OrdinalIgnoreCase))
                {
                    return item;
                }
            }

            return AuthentificationType.NONE;
        }
    }
}
