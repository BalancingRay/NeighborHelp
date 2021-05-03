using NeighborHelpModels.Models;
using NeighborHelpModels.Models.Consts;
using NeighborHelp.Services;
using NeighborHelp.Services.Contracts;
using NeighborHelpTests.Infrostructure;
using NUnit.Framework;
using System.Linq;

namespace NeighborHelpTests.Tests
{
    public class EntityOrderDirectoryTest : OrderDirectoryTestBase
    {
        private IUserDirectoryServise _userDirectoryInMemory;
        private IOrderDirectoryServise _orderDirectoryInMemory;
        private UserProfile _userProfile1;
        private UserProfile _userProfile2;
        protected override IUserDirectoryServise _UserDirectory => _userDirectoryInMemory;

        public override IOrderDirectoryServise OrderDirectory => _orderDirectoryInMemory;

        protected override UserProfile UserProfile1 => _userProfile1;
        protected override UserProfile UserProfile2 => _userProfile2;

        [SetUp]
        public void Setup()
        {
            var context = DataBaseBuilder.BuildDataBaseContext();
            var entityDirectory = new EntityUserOrderDirectory(context);
            _userDirectoryInMemory = entityDirectory;
            _orderDirectoryInMemory = entityDirectory;

            _userProfile1 = CreateDefaultUserProfile();
            _userProfile2 = CreateDefaultUserProfile(
                login: "user2", profileName: "Ivan", 
                address: "Prityckovo str.29", phoneNumber: "123-45-678");
        }

        [Test]
        public void PutOrderStatusTest_newDirectoryInstance()
        {
            bool trackingOption = true;
            string newStatus = OrderStatus.FINISHED;

            OrderDirectory.TryAddOrder(new Order() { Product = "prod", Author = UserProfile1 });

            var firstOrder = OrderDirectory.GetAllOrders(trackingOption).Single();
            firstOrder.Status = newStatus;
            bool result = OrderDirectory.TryPutOrder(firstOrder);

            var newDirectoryInstance = new EntityUserOrderDirectory(DataBaseBuilder.BuildDataBaseContext(false));
            firstOrder = newDirectoryInstance.GetAllOrders(trackingOption).Single();
            Assert.IsTrue(result == true
                && firstOrder.Status == newStatus);
        }
    }
}
