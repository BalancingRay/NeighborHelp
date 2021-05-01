using NeighborHelp.Models;
using NeighborHelp.Models.Consts;
using NeighborHelp.Services;
using NeighborHelp.Services.Contracts;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace NeighborHelpTests.Tests
{
    public abstract class OrderDirectoryTestBase
    {
        protected abstract IUserDirectoryServise _UserDirectory { get; }

        public abstract IOrderDirectoryServise OrderDirectory { get; }

        protected abstract UserProfile UserProfile1 { get; }

        protected abstract UserProfile UserProfile2 { get; }

        protected UserProfile CreateDefaultUserProfile(
            string login = "user1", string password = "12345", string profileName = "Andrei", 
            string phoneNumber = "100-200-300", string address = "Main str.128")
        {
            var user = new User()
            {
                Login = login,
                Password = password,
                Profile = new UserProfile()
                {
                    Name = profileName,
                    PhoneNumber = phoneNumber,
                    Address = address
                }
            };

            return CreateUserInDirectory(user).Profile;
        }

        protected User CreateUserInDirectory(User user)
        {
            _UserDirectory.TryAddUser(user);

            return _UserDirectory.GetUsers(true).FirstOrDefault(u => u.Login == user.Login);
        }

        #region AddOrder tests

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(10)]
        public void TryAddOrderTest(int countOfOrder)
        {
            bool result = true;

            for (int i = 1; i <= countOfOrder; i++)
            {
                result = OrderDirectory.TryAddOrder(new Order() {Product = $"product{i}", Author = UserProfile1});
            }

            var orders = OrderDirectory.GetAllOrders();

            Assert.IsTrue(result == true);
            Assert.IsTrue(orders.Count == countOfOrder);
        }

        [Test]
        public void UniqueOrderIdTest()
        {
            OrderDirectory.TryAddOrder(new Order() { Product = "vegetables", Author = UserProfile1 });
            OrderDirectory.TryAddOrder(new Order() { Product = "books", Author = UserProfile2 });

            var orders = OrderDirectory.GetAllOrders();

            Assert.IsTrue(orders[0].Id != orders[1].Id);
        }

        [Test]
        public void AddOrderAuthorByIdTest()
        {
            bool result = OrderDirectory.TryAddOrder(new Order() { Product = "bread", AuthorId = UserProfile2.Id });

            var order = OrderDirectory.GetAllOrders().Single();

            Assert.IsTrue(result == true
                && order.Author.Id == order.AuthorId);
        }

        [Test]
        public void CompareAutorIdTest()
        {
            bool result = OrderDirectory.TryAddOrder(new Order() { Product = "bread", Author = UserProfile1});

            var order = OrderDirectory.GetAllOrders().Single();

            Assert.IsTrue(result == true
                && order.Author.Id == order.AuthorId);
        }

        public void DenyAddDublicateOrdersTest()
        {
            var order = new Order() { Product = "vegetables", Author = UserProfile1 };
            OrderDirectory.TryAddOrder(new Order() );
            OrderDirectory.TryAddOrder(new Order() { Product = "books", Author = UserProfile2 });

            var orders = OrderDirectory.GetAllOrders();

            Assert.IsTrue(orders[0].Id != orders[1].Id);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public void DenyInvalidOrEmptyProductTest(string product)
        {
            bool result = OrderDirectory.TryAddOrder(new Order() { Product = product, Author = UserProfile1 });

            Assert.IsTrue(result == false);
            Assert.IsTrue(OrderDirectory.GetAllOrders().Count == 0);
        }

        [Test]
        public void DenyAddNullOrderTest()
        {
            bool result = OrderDirectory.TryAddOrder(null);

            Assert.IsTrue(result == false);
            Assert.IsTrue(OrderDirectory.GetAllOrders().Count == 0);
        }

        [Test]
        public void DenyAddNullUserProfileTest()
        {
            bool result = OrderDirectory.TryAddOrder(new Order() {Product="something" });

            Assert.IsTrue(result == false);
            Assert.IsTrue(OrderDirectory.GetAllOrders().Count == 0);
        }

        #endregion AddOrder tests

        #region GetOrder tests
        [Test]
        public void GetOrderProductTest()
        {
            string product = "cherry";

            OrderDirectory.TryAddOrder(new Order() { Product = product, Author = UserProfile1 });

            var orders = OrderDirectory.GetAllOrders();

            Assert.IsTrue(orders.Single().Product == product);
        }

        [Test]
        public void GetOrderDescriptionTest()
        {
            string description = "It's the best car in our city";

            OrderDirectory.TryAddOrder(new Order() { Product="car", ProductDescription = description, Author = UserProfile1 });

            var orders = OrderDirectory.GetAllOrders();

            Assert.IsTrue(orders.Single().ProductDescription == description);
        }

        [Test]
        public void GetOrderStatusTest()
        {
            string status = OrderStatus.ACTIVE;

            OrderDirectory.TryAddOrder(new Order() {Status = status, Product = "car", Author = UserProfile1 });

            var orders = OrderDirectory.GetAllOrders();

            Assert.IsTrue(orders.Single().Status == status);
        }

        [Test]
        public void GetDefaultOrderStatusTest()
        {
            OrderDirectory.TryAddOrder(new Order() {Product = "car", Author = UserProfile1 });

            var orders = OrderDirectory.GetAllOrders();

            Assert.IsTrue(orders.Single().Status == OrderStatus.INITIALIZE);
        }

        [Test]
        public void GetOrderTypeTest()
        {
            string orderType = OrderTypes.SELL;

            OrderDirectory.TryAddOrder(new Order() { OrderType = orderType, Product = "car", Author = UserProfile1 });

            var orders = OrderDirectory.GetAllOrders();

            Assert.IsTrue(orders.Single().OrderType == orderType);
        }

        [Test]
        public void GetOrderCostTest()
        {
            double orderCost = 6800f;

            OrderDirectory.TryAddOrder(new Order() { Cost = orderCost, Product = "car", Author = UserProfile1 });

            var orders = OrderDirectory.GetAllOrders();

            Assert.IsTrue(orders.Single().Cost == orderCost);
        }

        [Test]
        public void GetOrdersByUserIdTest()
        {
            OrderDirectory.TryAddOrder(new Order() { Author = UserProfile1, Product = "user_1_prod_1"});
            OrderDirectory.TryAddOrder(new Order() { Author = UserProfile1, Product = "user_1_prod_2" });            
            
            OrderDirectory.TryAddOrder(new Order() { Author = UserProfile2, Product = "user_2_prod_1" });

            var user1orders = OrderDirectory.GetOrders(UserProfile1.Id);
            Assert.IsTrue(user1orders.Count == 2);

            var user2orders = OrderDirectory.GetOrders(UserProfile2.Id);
            Assert.IsTrue(user2orders.Count == 1);

            var allOrders = OrderDirectory.GetAllOrders();
            Assert.IsTrue(allOrders.Count == 3);
        }

        [Test]
        public void GetOrderByIdTest()
        {
            string product = "apple";

            OrderDirectory.TryAddOrder(new Order() { Product = product, Author = UserProfile1 });

            int orderId = OrderDirectory.GetAllOrders().First().Id;
            var order = OrderDirectory.GetOrder(orderId);

            Assert.IsTrue(order.Product == product);
        }

        #endregion GetOrder tests

        #region Tracking tests

        [Test]
        public void Get_tracked_ChangingProductTest()
        {
            bool trackingOption = true;

            string originalProduct = "cherry"; 
            string newProduct = "banana";

            OrderDirectory.TryAddOrder(new Order() { Product = originalProduct, Author = UserProfile1 });

            OrderDirectory.GetAllOrders(trackingOption).Single()
                .Product = newProduct;
            var order = OrderDirectory.GetAllOrders(trackingOption).Single();

            Assert.IsTrue(order.Product == newProduct);
        }

        [Test]
        public void Get_untracked_ChangingProductTest()
        {
            bool trackingOption = false;

            string originalProduct = "cherry";
            string newProduct = "banana";

            OrderDirectory.TryAddOrder(new Order() { Product = originalProduct, Author = UserProfile1 });

            OrderDirectory.GetAllOrders(trackingOption).Single()
                .Product = newProduct;
            var order = OrderDirectory.GetAllOrders(trackingOption).Single();

            Assert.IsTrue(order.Product == originalProduct);
        }

        #endregion Tracking tests

        #region PutUser Tests

        ////TODO Why this test not working with Entity Framework in case during tracking disabled?
        [TestCase(true)]
        //[TestCase(false)]
        public void PutOrderProductTest_TrackingOption(bool trackingOption)
        {
            string originalProduct = "first";
            string newProduct = "newProd";
            OrderDirectory.TryAddOrder(new Order() { Product = originalProduct, Author = UserProfile1 });         


            var firstOrder = OrderDirectory.GetAllOrders(trackingOption).Single();
            firstOrder.Product = newProduct;
            bool result = OrderDirectory.TryPutOrder(firstOrder);

            firstOrder = OrderDirectory.GetAllOrders(trackingOption).Single();
            Assert.IsTrue(result == true
                && firstOrder.Product == newProduct);
        }

        ////TODO Why this test not working with Entity Framework in case during tracking disabled?
        [TestCase(true)]
        //[TestCase(false)]
        public void PutOrderStatusTest_TrackingOption(bool trackingOption)
        {
            string originalStatus = OrderStatus.INITIALIZE;
            string newStatus = OrderStatus.ACTIVE;

            OrderDirectory.TryAddOrder(new Order() { Status = originalStatus, Product = "prod", Author = UserProfile1 });

            var firstOrder = OrderDirectory.GetAllOrders(trackingOption).Single();
            firstOrder.Status = newStatus;
            bool result = OrderDirectory.TryPutOrder(firstOrder);

            firstOrder = OrderDirectory.GetAllOrders(trackingOption).Single();
            Assert.IsTrue(result == true
                && firstOrder.Status == newStatus);
        }

        [Test]
        public void DenyPutNewOrderTest()
        {
            bool result = OrderDirectory.TryPutOrder(new Order() { Product = "thomething", Author = UserProfile1 });

            var orderCount = OrderDirectory.GetAllOrders().Count;

            Assert.IsTrue(result == false);
            Assert.IsTrue(orderCount == 0);
        }

        //Directory don't have any mechanism to protect order.id from changing manyally.
        //TODO fix them
        //[Test]
        public void DenyChangeOrderIdTest()
        {
            bool trackingOption = true;

            OrderDirectory.TryAddOrder(new Order() { Product = "first", Author = UserProfile1 });
            OrderDirectory.TryAddOrder(new Order() { Product = "second", Author = UserProfile1 });
            string newProduct = "third";

            var firstOrder = OrderDirectory.GetAllOrders(trackingOption)[0];
            firstOrder.Id++;
            firstOrder.Product = newProduct ;
            bool result = OrderDirectory.TryPutOrder(firstOrder);

            firstOrder = OrderDirectory.GetAllOrders(trackingOption)[0];
            Assert.IsTrue(result == false);
            Assert.IsTrue(firstOrder.Product != newProduct);
        }

        #endregion PutUser tests
    }
}