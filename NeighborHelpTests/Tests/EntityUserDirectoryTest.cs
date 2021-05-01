using NeighborHelp.Models;
using NeighborHelp.Services;
using NeighborHelp.Services.Contracts;
using NeighborHelpTests.Infrostructure;
using NUnit.Framework;
using System.Linq;

namespace NeighborHelpTests.Tests
{
    public class EntityUserDirectoryTest : UserDirectoryTestBase
    {
        private IUserDirectoryServise _userDirectoryInDataBase;
        public override IUserDirectoryServise UserDirectory => _userDirectoryInDataBase;

        [SetUp]
        public void Setup()
        {
            var context = DataBaseBuilder.BuildDataBaseContext();
            _userDirectoryInDataBase = new EntityUserOrderDirectory(context);
        }

        [Test]
        public void PutUserNameTest_newDirectoryInstance()
        {
            bool useTracking = true;
            UserDirectory.TryAddUser(new User() { Login = "user1" });
            string userName = "TestName";

            var firstUser = UserDirectory.GetUsers(useTracking).Single();
            firstUser.UserName = userName;
            bool result = UserDirectory.TryPutUser(firstUser);

            var newDirectoryInstance = new EntityUserOrderDirectory(DataBaseBuilder.BuildDataBaseContext(false));
            firstUser = newDirectoryInstance.GetUsers(useTracking).Single();
            Assert.IsTrue(result && firstUser.UserName == userName);
        }

        //TODO add tests to chekc results in another dataContext
    }
}
