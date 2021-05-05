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
        public static void InitializeBoolProperty(this IConfiguration configuration, string propertyName, ref bool value)
        {
            string textValue = configuration.GetSection(propertyName)?.Value;

            if (!string.IsNullOrEmpty(textValue))
            {
                value = textValue == "true";
            }
        }

        public static bool ReadBoolProperty(this IConfiguration configuration, string propertyName, bool defaultValue = false)
        {
            string textValue = configuration.GetSection(propertyName)?.Value;

            if (string.IsNullOrEmpty(textValue))
                return defaultValue;

            return textValue == "true";

        }

        public static AuthentificationType ReadAuthentificationType(this IConfiguration configuration)
        {
            string textValue = configuration.GetSection(nameof(AuthentificationType))?.Value;

            var data = Enum.GetValues<AuthentificationType>();

            foreach (var item in data)
            {
                if (textValue?.ToUpper() == item.ToString().ToUpper())
                {
                    return item;
                }
            }

            return AuthentificationType.NONE;
        }
    }
}
