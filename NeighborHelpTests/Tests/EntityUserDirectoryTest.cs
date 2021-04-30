using NeighborHelp.Services;
using NeighborHelpTests.Infrostructure;
using NUnit.Framework;

namespace NeighborHelpTests.Tests
{
    public class EntityUserDirectoryTest : UserDirectoryTestBase
    {
        [SetUp]
        public void Setup()
        {
            var context = DataBaseBuilder.BuildDataBaseContext();
            UserDirectory = new EntityUserOrderDirectory(context);
        }

        //TODO add tests to chekc results in another dataContext
    }
}
