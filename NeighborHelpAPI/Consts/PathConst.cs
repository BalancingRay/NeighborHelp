namespace NeighborHelpAPI.Consts
{
    public static class PathConst
    {
        private const string API_AREA = "API";
        private const string USER_CONTROLLER = "USER";
        private const string ORDER_CONTROLLER = "ORDER";
        private const string LOGIN_CONTROLLER = "Authentification";

        public static readonly string LOGIN_BY_JWT_PATH = $"/{LOGIN_CONTROLLER}/{AuthenticationConsts.LOGIN_BY_JWT}";
        public static readonly string LOGIN_BY_COOKIES_PATH = $"/{LOGIN_CONTROLLER}/{AuthenticationConsts.LOGIN_BY_COOKIES}";

        private static readonly string USER_API = $"/{API_AREA}/{USER_CONTROLLER}/";

        public static readonly string ADD_USER_PATH = $"{USER_API}{UserControllerConsts.ADD_USER}";
        public static readonly string CURRENT_USER_PATH = $"{USER_API}{UserControllerConsts.GET_CURRENT_ACTION}";
        public static readonly string GET_USER_PATH = $"{USER_API}{UserControllerConsts.GET_ACTION}";
        public static readonly string GET_USERS_PATH = $"{USER_API}{UserControllerConsts.GET_ALL_ACTION}";
        public static readonly string PUT_USER_PATH = $"{USER_API}{UserControllerConsts.UPDATE_ACTION}";

        private static readonly string ORDER_API = $"/{API_AREA}/{ORDER_CONTROLLER}/";

        public static readonly string ADD_ORDER_PATH = $"{ORDER_API}{OrderControllerConsts.ADD_ACTION}";
        public static readonly string GET_ORDER_PATH = $"{ORDER_API}{OrderControllerConsts.GET_ACTION}";
        public static readonly string GET_ORDERS_PATH = $"{ORDER_API}{OrderControllerConsts.GET_ALL_ACTION}";
        public static readonly string GET_ORDERS_BY_USER_PATH = $"{ORDER_API}{OrderControllerConsts.GET_BY_USER_ACTION}";
        public static readonly string PUT_ORDER_PATH = $"{ORDER_API}{OrderControllerConsts.PUT_ACTION}";
        public static readonly string RESPONCE_ORDER_PATH = $"{ORDER_API}{OrderControllerConsts.RESPONSE_ACTION}";
    }
}
