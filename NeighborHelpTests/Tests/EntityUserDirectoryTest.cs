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
        private EntityUserOrderDirectory _directoryService;
        public override IUserDirectoryServise UserDirectory => _directoryService;

        public override IOrderDirectoryServise OrderDirectory => _directoryService;

        [SetUp]
        public void Setup()
        {
            var context = DataBaseBuilder.BuildDataBaseContext();
            _directoryService = new EntityUserOrderDirectory(context);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void PutUserNameTest_newDirectoryInstance(bool tracking)
        {
            //Arrange
            UserDirectory.TryAddUser(new User() { Login = "user1" });
            string userName = "TestName";
            var user = UserDirectory.GetUsers(tracking).Single();

            //Act
            user.UserName = userName;
            bool result = UserDirectory.TryPutUser(user);

            //Assert
            IUserDirectoryServise newDirectoryInstance = new EntityUserOrderDirectory(DataBaseBuilder.BuildDataBaseContext(false));
            user = newDirectoryInstance.GetUsers(tracking).Single();
            Assert.IsTrue(result 
                && user.UserName == userName);
        }

        //TODO add tests to chekc results in another dataContext
    }
}
