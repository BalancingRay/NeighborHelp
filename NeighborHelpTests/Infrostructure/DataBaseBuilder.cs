using Microsoft.EntityFrameworkCore;
using NeighborHelp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeighborHelpTests.Infrostructure
{
    internal static class DataBaseBuilder
    {
        private static bool useInMemoryImplementation = true;

        private const string IN_MEMORY_SERVER_NAME = "NeighborHelpTestDataBaseInMemory";
        private const string SQL_TEST_SERVER_SETTINGS = @"Server=(localdb)\mssqllocaldb;Database=TestDataBase;Trusted_Connection=True;";

        internal static ApplicationContext BuildDataBaseContext(bool clearOldData = true)
        {

            var options = new DbContextOptionsBuilder<ApplicationContext>()
                    .UseSqlServer(SQL_TEST_SERVER_SETTINGS)
                    .Options;

            if (useInMemoryImplementation)
            {
                options = new DbContextOptionsBuilder<ApplicationContext>()
                    .UseInMemoryDatabase(IN_MEMORY_SERVER_NAME)
                    .Options;
            }

            return new ApplicationContext(options, clearOldData);
        }
    }
}
