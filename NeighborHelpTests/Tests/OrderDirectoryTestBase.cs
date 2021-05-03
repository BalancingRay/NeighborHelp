using NeighborHelpModels.Models;
using NeighborHelpModels.Models.Consts;
using NeighborHelp.Services.Contracts;
using NUnit.Framework;
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

            return _UserDirectory.GetUsers().FirstOrDefault(u => u.Login == user.Login);
        }

        #region AddOrder tests

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(10)]
        public void AddOrder(int countOfOrder)
        {
            bool result = true;

            for (int i = 1; i <= countOfOrder; i++)
            {
                result = OrderDirectory.TryAddOrder(new Order() { Product = $"product{i}", Author = UserProfile1 });
            }

            var orders = OrderDirectory.GetAllOrders();

            Assert.IsTrue(result == true);
            Assert.IsTrue(orders.Count == countOfOrder);
        }

        [Test]
        public void UniqueOrderId()
        {
            OrderDirectory.TryAddOrder(new Order() { Product = "vegetables", Author = UserProfile1 });
            OrderDirectory.TryAddOrder(new Order() { Product = "books", Author = UserProfile2 });

            var orders = OrderDirectory.GetAllOrders();

            Assert.IsTrue(orders[0].Id != orders[1].Id);
        }

        [Test]
        public void AddOrderAuthorById()
        {
            bool result = OrderDirectory.TryAddOrder(new Order() { Product = "bread", AuthorId = UserProfile2.Id });

            var order = OrderDirectory.GetAllOrders().Single();

            Assert.IsTrue(result == true
                && order.Author.Id == order.AuthorId);
        }

        [Test]
        public void CompareAutorId()
        {
            bool result = OrderDirectory.TryAddOrder(new Order() { Product = "bread", AuthorId = UserProfile1.Id });

            var order = OrderDirectory.GetAllOrders().Single();

            Assert.IsTrue(result == true
                && order.Author.Id == order.AuthorId);
        }

        public void Deny_AddDublicateOrders()
        {
            var order = new Order() { Product = "vegetables", Author = UserProfile1 };
            OrderDirectory.TryAddOrder(new Order());
            OrderDirectory.TryAddOrder(new Order() { Product = "books", Author = UserProfile2 });

            var orders = OrderDirectory.GetAllOrders();

            Assert.IsTrue(orders[0].Id != orders[1].Id);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public void Deny_InvalidOrEmptyProduct(string product)
        {
            bool result = OrderDirectory.TryAddOrder(new Order() { Product = product, Author = UserProfile1 });

            Assert.IsTrue(result == false);
            Assert.IsTrue(OrderDirectory.GetAllOrders().Count == 0);
        }

        [Test]
        public void Deny_AddNullOrder()
        {
            bool result = OrderDirectory.TryAddOrder(null);

            Assert.IsTrue(result == false);
            Assert.IsTrue(OrderDirectory.GetAllOrders().Count == 0);
        }

        [Test]
        public void Deny_AddOrder_withot_Author()
        {
            bool result = OrderDirectory.TryAddOrder(new Order() { Product = "something" });

            Assert.IsTrue(result == false);
            Assert.IsTrue(OrderDirectory.GetAllOrders().Count == 0);
        }

        [Test]
        public void Deny_AddOrder_with_new_Author()
        {
            bool result = OrderDirectory.TryAddOrder(new Order() { Product = "something", Author = new UserProfile() });

            Assert.IsTrue(result == false);
            Assert.IsTrue(OrderDirectory.GetAllOrders().Count == 0);
        }

        [Test]
        public void Deny_AddOrder_with_incorrect_AuthorId()
        {
            bool result = OrderDirectory.TryAddOrder(new Order() { Product = "something", AuthorId = 20 });

            Assert.IsTrue(result == false);
            Assert.IsTrue(OrderDirectory.GetAllOrders().Count == 0);
        }

        #endregion AddOrder tests

        #region GetOrder tests
        [Test]
        public void GetOrderProduct()
        {
            string product = "cherry";

            OrderDirectory.TryAddOrder(new Order() { Product = product, AuthorId = UserProfile1.Id });

            var orders = OrderDirectory.GetAllOrders();

            Assert.IsTrue(orders.Single().Product == product);
        }

        [Test]
        public void GetOrderDescription()
        {
            string description = "It's the best car in our city";

            OrderDirectory.TryAddOrder(new Order() { Product = "car", ProductDescription = description, Author = UserProfile1 });

            var orders = OrderDirectory.GetAllOrders();

            Assert.IsTrue(orders.Single().ProductDescription == description);
        }

        [Test]
        public void GetOrderStatus()
        {
            string status = OrderStatus.ACTIVE;

            OrderDirectory.TryAddOrder(new Order() { Status = status, Product = "car", Author = UserProfile1 });

            var orders = OrderDirectory.GetAllOrders();

            Assert.IsTrue(orders.Single().Status == status);
        }

        [Test]
        public void GetDefaultOrderStatus()
        {
            OrderDirectory.TryAddOrder(new Order() { Product = "car", Author = UserProfile1 });

            var orders = OrderDirectory.GetAllOrders();

            Assert.IsTrue(orders.Single().Status == OrderStatus.INITIALIZE);
        }

        [Test]
        public void GetOrderType()
        {
            string orderType = OrderTypes.SELL;

            OrderDirectory.TryAddOrder(new Order() { OrderType = orderType, Product = "car", Author = UserProfile1 });

            var orders = OrderDirectory.GetAllOrders();

            Assert.IsTrue(orders.Single().OrderType == orderType);
        }

        [Test]
        public void GetOrderCost()
        {
            double orderCost = 6800f;

            OrderDirectory.TryAddOrder(new Order() { Cost = orderCost, Product = "car", Author = UserProfile1 });

            var orders = OrderDirectory.GetAllOrders();

            Assert.IsTrue(orders.Single().Cost == orderCost);
        }

        [Test]
        public void GetOrders_ByAuthorId()
        {
            OrderDirectory.TryAddOrder(new Order() { Author = UserProfile1, Product = "user_1_prod_1" });
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
        public void GetOrder_ById()
        {
            string product = "apple";

            OrderDirectory.TryAddOrder(new Order() { Product = product, AuthorId = UserProfile1.Id });

            int orderId = OrderDirectory.GetAllOrders().First().Id;
            var order = OrderDirectory.GetOrder(orderId);

            Assert.IsTrue(order.Product == product);
        }

        #endregion GetOrder tests

        #region Tracking tests

        [Test]
        public void Get_tracked_ChangingProduct()
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
        public void Get_untracked_ChangingProduct()
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

        #region PutOrder tests

        [TestCase(true)]
        [TestCase(false)]
        public void PutOrderProduct_trackingOption(bool trackingOption)
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

        [TestCase(true)]
        [TestCase(false)]
        public void PutOrderStatus_trackingOption(bool trackingOption)
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
        public void Deny_PutNewOrder()
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
            firstOrder.Product = newProduct;
            bool result = OrderDirectory.TryPutOrder(firstOrder);

            firstOrder = OrderDirectory.GetAllOrders(trackingOption)[0];
            Assert.IsTrue(result == false);
            Assert.IsTrue(firstOrder.Product != newProduct);
        }

        #endregion PutOrder tests

        #region RemoveOrder tests

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void RemoveOrder(int number)
        {
            //Arrange
            OrderDirectory.TryAddOrder(new Order() { Product = "prod1", Author = UserProfile1 });
            OrderDirectory.TryAddOrder(new Order() { Product = "prod2", Author = UserProfile2 });
            OrderDirectory.TryAddOrder(new Order() { Product = "prod3", Author = UserProfile2 });
            OrderDirectory.TryAddOrder(new Order() { Product = "prod4", Author = UserProfile1 });

            //Act
            var order = OrderDirectory.GetAllOrders()[number];
            bool result = OrderDirectory.TryRemoveOrder(order.Id);

            //Assert
            Assert.IsTrue(result == true);
            Assert.IsTrue(OrderDirectory.GetAllOrders().Count() == 3);
            Assert.IsFalse(OrderDirectory.GetAllOrders().Any(o => o.Id == order.Id || o.Product == order.Product));
        }

        [Test]
        public void Deny_RemoveOrder_by_incorrect_Id()
        {
            //Arrange
            OrderDirectory.TryAddOrder(new Order() { Product = "prod1", Author = UserProfile1 });

            //Act
            var order = OrderDirectory.GetAllOrders().Single();
            bool result = OrderDirectory.TryRemoveOrder(order.Id + 25);

            //Assert
            Assert.IsFalse(result);
            Assert.IsFalse(OrderDirectory.GetAllOrders().Count() == 0);
            Assert.IsTrue(OrderDirectory.GetAllOrders().Any(o => o.Id == order.Id || o.Product == order.Product));
        }

        #endregion RemoveOrder tests
    }
}