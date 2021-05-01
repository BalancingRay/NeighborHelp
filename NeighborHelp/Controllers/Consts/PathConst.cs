using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeighborHelp.Controllers.Consts
{
    public static class PathConst
    {
        public const string API_AREA = "API";
        public const string USER_CONTROLLER = "USER";
        public const string ORDER_CONTROLLER = "ORDER";
        public const string LOGIN_CONTROLLER = "Authentification";
        public static string LOGIN_PATH = $"/{LOGIN_CONTROLLER}/Login";
    }
}
