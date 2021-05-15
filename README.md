# NeighborHelp

**The project main goals are**

- to have a practice of creating a server application for different types of clients,

- to use new technologies,

- to follow clean architecture and clean code approaches.

## Server functionality
- User registration. Update user profile.
- Authentication both with Cookies or/and Json Web Token schemes
- Different rights to use API following with user Role.
- Creating, reading and updating models (User and Order) by WebAPI
- Chat beetween clients based on SignalR

Also there are following simple clients:
- WebClient by html pages with JS - [/master/NeighborHelpWebClient](https://github.com/BalancingRay/NeighborHelp/tree/master/NeighborHelpWebClient)
- Xamarin client - [github.com/BalancingRay/NeighborHelpMobileClient](https://github.com/BalancingRay/NeighborHelpMobileClient)

## Tehnologies
- [.NET 5](https://docs.microsoft.com/en-us/dotnet/core/dotnet-five)
- [ASP.NET core 5](https://docs.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-5.0)
- [Entity Framework Core 5](https://docs.microsoft.com/en-us/ef/core/)
- [WEB API](https://docs.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-5.0)
- [SignalR Core](https://docs.microsoft.com/en-us/aspnet/core/signalr/introduction?view=aspnetcore-5.0)
- [NUnit](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-nunit)

## Practices
- Clean Architecture
- Clean Code
- SOLID Principles
- Separation of Concerns
- Unit testing

## Structure of projects:
### [NeighborHelpModels](https://github.com/BalancingRay/NeighborHelp/tree/master/NeighborHelpModels)
Data Transfer Objects and their constants. Extentions to compare, modify and dublicate models. Use on both sides: clients and server.
```cs
     public class Order
    {
        public int Id { get; set; }
        public int AuthorId { set; get; }
        public UserProfile Author { set; get; }

        public string Product { set; get; }
        public string ProductDescription { set; get; }
        public double Cost { set; get; }
        public string OrderType { set; get; }

        public string Status { get; set; }
        public int ClientId { set; get; }
    }
```
### [NeighborHelpAPI](https://github.com/BalancingRay/NeighborHelp/tree/master/NeighborHelpAPI)
Names of controllers and their actions, which used for routing on the server side. Use by clients to get API paths and chatHub commands.
```cs
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
```
### [NeighborHelpInfrastructure](https://github.com/BalancingRay/NeighborHelp/tree/master/NeighborHelpInfrastructure)
Interfaces of services for working with models. Common utils for authorization and configuration
```cs
    public interface IUserDirectoryServise
    {
        public User GetUser(string login, string password);
        public User GetUser(int id, bool useTracking = ContractConsts.DefaultTracking);
        public IList<User> GetUsers(bool useTracking = ContractConsts.DefaultTracking);
        public bool TryAddUser(User user);
        public bool TryPutUser(User user);
        public bool TryRemoveUser(int id, bool removeRelatedOrders);
    }
```
### [NeighborHelp](https://github.com/BalancingRay/NeighborHelp/tree/master/NeighborHelp)
Initialization and registration services, dataBase and Build middlewares. Include main services implementations, extentions for building services and middlewares, controllers.
```cs
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            string authArea = AuthenticationConfigurationExtention.ConfigurationArea;
            string dbArea = StartupDataBaseExtention.ConfigurationArea;
            services.ConfigureControllers();
            services.ConfigureChatHub();
            services.ConfigureAuthentication(Configuration.GetSection(authArea));
            services.ConfigureDirectoryServices(Configuration.GetSection(dbArea));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseWebClientStaticFiles(Configuration, env);
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapChatHub();
            });
        }
    }
```
```cs
    public class ApplicationContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options, bool clearOldData = false) :base(options)
        {
            if (clearOldData)
            {
                Database.EnsureDeleted();
            }
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
		.HasOne(u => u.Profile).WithOne(p => p.OwnerUser)
	        .HasForeignKey<UserProfile>(up => up.Id);

            modelBuilder.Entity<Order>()
		.HasOne(o => o.Author).WithMany(a => a.Orders)
		.HasForeignKey(o => o.AuthorId);
        }
    }
```
```cs
    public class EntityUserOrderDirectory : IOrderDirectoryServise, IUserDirectoryServise
    {
	protected ApplicationContext DataBase { get; }
        protected DbSet<User> Users => DataBase.Users;

        #region IUserDirectoryServise implementation

        public bool TryAddUser(User user)
        {
            if (user == null)
                return false;

            bool isLoginEmpty = string.IsNullOrWhiteSpace(user.Login);
            bool isLoginExist = Users.Any(u => u.Login == user.Login);
            bool isProfileDublicated = user.Profile != null && Users.Any(u => u.Profile == user.Profile);

            if (isLoginEmpty || isLoginExist || isProfileDublicated)
            {
                return false;
            }
            else
            {
                Users.Add(user);
                DataBase.SaveChanges();

                return true;
            }
        }

        public User GetUser(int id, bool useTraching)
        {
            if (useTraching)
            {
                return Users.Include(u => u.Profile).SingleOrDefault(u => u.Id == id);
            }
            else
            {
                return Users.AsNoTracking().Include(u => u.Profile).SingleOrDefault(u => u.Id == id);
            }
        }

        public bool TryPutUser(User user)
        {
            if (Users.Any(u => u.Login == user.Login && u.Id == user.Id))
            {
                if (IsDetached(user))
                {
                    var originalUser = GetUser(user.Id, true);
                    originalUser.UpdateFrom(user);
                }
                DataBase.SaveChanges();

                return true;
            }
            return false;
        }
       ...
```
```cs
    [ApiController]
    [Route("api/[Controller]/[Action]")]
    public class UserController : Controller
    {
        private IUserDirectoryServise _userDirectory;

        public UserController(IUserDirectoryServise service)
        {
            _userDirectory = service;
        }

        [HttpGet]
        [ActionName(UserControllerConsts.GET_CURRENT_ACTION)]
        [Authorize(AuthenticationSchemes = AuthorizeAttributeHelper.Value)]
        public ActionResult<User> Current()
        {
            User user = null;
            if (AuthorizationHelper.TryGetCurrentUserId(HttpContext?.User, out int id))
            {
                user = _userDirectory.GetUser(id);
            }

            if (user != null)
            {
                return new ActionResult<User>(user);
            }
            else
            {
                return new NotFoundResult();
            }
        }

        [HttpGet("{id}")]
        [ActionName(UserControllerConsts.GET_ACTION)]
        [Authorize(Roles = UserRoles.ADMIN)]
        [Authorize(AuthenticationSchemes = AuthorizeAttributeHelper.Value)]
        public ActionResult<User> Get(int id)
        {
            var user = _userDirectory.GetUser(id);

            if (user != null)
            {
                return new ActionResult<User>(user);
            }
            else
            {
                return new NotFoundResult();
            }
        }
        ...
```
### [NeighborHelpChat](https://github.com/BalancingRay/NeighborHelp/tree/master/NeighborHelpChat)
ChatHub and its services. Include services registration and routing extentions for Startup method.
```cs
    [Authorize(AuthenticationSchemes = AuthorizeAttributeHelper.Value)]
    public class ChatHub : Hub
    {
        private IChatUserProvider users;
        public string CurrentUserName => users.GetCurrentUserName(Context);

        public ChatHub(IUserDirectoryServise userService)
        {
            users = new ChatUserProvider(userService);
        }

        [HubMethodName(ChatHubConsts.SendMessage)]
        public async Task Send(string message)
        {
            string username = users.GetCurrentUserName(Context);
            await Clients.All.SendAsync(ChatHubConsts.ReceiveClientsMesage, message, CurrentUserName);
        }

        [HubMethodName(ChatHubConsts.SendToGroup)]
        public async Task SendToGroup(string message, string group)
        {
            await Clients.Group(group).SendAsync(ChatHubConsts.ReceiveClientsMesage, message, CurrentUserName);
        }
        ...
```
### [NeighborHelpWebClient](https://github.com/BalancingRay/NeighborHelp/tree/master/NeighborHelpWebClient)
Static html files and js scripts. Include middleware registration extention.
### [NeighborHelpTests](https://github.com/BalancingRay/NeighborHelp/tree/master/NeighborHelpTests)
Unit tests for services and controllers
