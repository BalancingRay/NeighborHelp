using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NeighborHelp.Services;
using NeighborHelpInfrastructure.ServiceContracts;
using NeighborHelpInfrastucture.Utils;

namespace NeighborHelp.Utils
{
    internal static class StartupDataBaseExtention
    {
        public const string ConfigurationArea = "DataBase";

        private const string InMemotyDbPropertyName = "UseInMemotyDB";
        private const string AddTestDataPropertyName = "UseTestData";
        private const string ClearDbOnStartPropertyName = "ClearDBOnStart";
        private const string ConnectionPropertyName = "DefaultConnection";

        internal static IServiceCollection ConfigureDirectoryServices(this IServiceCollection services, IConfiguration DirectoryConfiguration)
        {
            bool useInMemotyDB = DirectoryConfiguration.ReadBoolProperty(InMemotyDbPropertyName);

            if (useInMemotyDB)
            {
                services.ConfigureInMemoryDataBase(DirectoryConfiguration);
            }
            else
            {
                services.ConfigureSQLDataBase(DirectoryConfiguration);
            }
            return services;
        }

        private static void ConfigureInMemoryDataBase(this IServiceCollection services, IConfiguration DirectoryConfiguration)
        {
            var directoryService = new MemoryUserOrderDirectory();

            services.AddSingleton(typeof(IOrderDirectoryServise), directoryService);
            services.AddSingleton(typeof(IUserDirectoryServise), directoryService);

            bool addTestData = DirectoryConfiguration.ReadBoolProperty(AddTestDataPropertyName);

            if (addTestData)
            {
                var testData = new TestDataFiller(directoryService, directoryService);
                testData.FillIfEmpty();
            }
        }

        private static void ConfigureSQLDataBase(this IServiceCollection services, IConfiguration DirectoryConfiguration)
        {
            bool clearDBOnStart = DirectoryConfiguration.ReadBoolProperty(ClearDbOnStartPropertyName);
            bool addTestData = DirectoryConfiguration.ReadBoolProperty(AddTestDataPropertyName);
            string connection = DirectoryConfiguration.GetConnectionString(ConnectionPropertyName);

            var options = new DbContextOptionsBuilder<ApplicationContext>()
                    .UseSqlServer(connection)
                    .Options;

            services.AddScoped(typeof(IOrderDirectoryServise),
                (servProvider) =>
                new EntityUserOrderDirectory(new ApplicationContext(options)));
            services.AddScoped(typeof(IUserDirectoryServise),
                (servProvider) =>
                new EntityUserOrderDirectory(new ApplicationContext(options)));

            if (addTestData)
            {
                ApplicationContext applicationContext = new ApplicationContext(options, clearDBOnStart);
                var directoryService = new EntityUserOrderDirectory(applicationContext);
                var testData = new TestDataFiller(directoryService, directoryService);
                testData.FillIfEmpty();
            }
            else if (clearDBOnStart)
            {
                new ApplicationContext(options, clearDBOnStart);
            }
        }
    }
}
