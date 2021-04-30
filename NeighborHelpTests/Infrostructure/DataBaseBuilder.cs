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
        private const string TEST_SERVER_SETTINGS = @"Server=(localdb)\mssqllocaldb;Database=TestDataBase;Trusted_Connection=True;";

        internal static ApplicationContext BuildDataBaseContext()
        {

            var options = new DbContextOptionsBuilder<ApplicationContext>()
                    .UseSqlServer(TEST_SERVER_SETTINGS)
                    .Options;

            return new ApplicationContext(options, true);
        }
    }
}
