using Microsoft.AspNetCore.Mvc;
using NeighborHelp.Controllers;
using NeighborHelpModels.Models;
using NeighborHelp.Services;
using NeighborHelp.Services.Contracts;
using NeighborHelpTests.Infrostructure;
using NUnit.Framework;
using System;
using System.Linq;

namespace NeighborHelpTests.Tests
{
    public class OrderControllerTest
    {
        private bool useSQLServer = true;
        private bool useMockDirectory = false;

        private OrderController _orderController;
        private IUserDirectoryServise _userDirectoryService;
        private IOrderDirectoryServise _orderDirectoryService;

        private User _defaultUser;

        private OrderController OrderController => _orderController;
        private IUserDirectoryServise UserDirectoryService => _userDirectoryService;
        private IOrderDirectoryServise OrderDirectoryService => _orderDirectoryService;

        private User DefaultUser => _defaultUser;

        [SetUp]
        public void Setup()
        {
            if (useMockDirectory)
            {
                _userDirectoryService = null;
                //TODO Initialize mock directory
                throw new NotImplementedException("MockDirectory have not implemented yet");
            }
            else if (useSQLServer)
            {
                var context = DataBaseBuilder.BuildDataBaseContext();
                var service = new EntityUserOrderDirectory(context);
                _userDirectoryService = service;
                _orderDirectoryService = service;
            }
            else
            {
                var service = new MemoryUserOrderDirectory();
                _userDirectoryService = service;
                _orderDirectoryService = service;
            }

            AddTwoUsers();
            _defaultUser = UserDirectoryService.GetUsers().First();

            _orderController = new OrderController(_orderDirectoryService);
        }

        private void AddTwoUsers()
        {
            UserDirectoryService.TryAddUser(new User()
            {
                Login = $"user1",
                Role = "user",
                Profile = new UserProfile()
                {
                    Name = "Mark",
                    Address = "Berlin"
                }
            });

            UserDirectoryService.TryAddUser(new User()
            {
                Login = $"user2",
                Role = "admin",
                Profile = new UserProfile()
                {
                    Name = "Stephen",
                    Address = "London"
                }
            });
        }

        [Test]
        public void PostOrder_after_Authentificate()
        {
            //Arrange
            string productName = "someProd";
            string description = "some details";
            OrderController.ControllerContext = ControllerContextAuthentificator.Authentificate(DefaultUser);

            //Act
            var responce = OrderController.Post(new Order() { Product = productName, ProductDescription = description });

            //Assert
            var order = _orderDirectoryService.GetAllOrders().Single();
            Assert.IsTrue(responce.Value.Product == productName);
            Assert.IsTrue(order.Product == productName);
        }

        [Test]
        public void Deny_PostOrder_without_Authentificate()
        {
            //Act
            var responce = OrderController.Post(new Order() { Product = "someProd", Author = DefaultUser.Profile});

            //Assert
            var orders = _orderDirectoryService.GetAllOrders();
            Assert.IsTrue(orders.Count() == 0);
            Assert.IsNull(responce.Value);
            Assert.IsInstanceOf(typeof(BadRequestResult), responce.Result, $"Result type should be BadRequestResult, by current is {0}", responce.Result.GetType());
        }


        [TestCase(0)]
        [TestCase(2)]
        [TestCase(10)]
        public void GetAllOrders(int dbOrders_count)
        {
            //Arrange
            for (int i = 1; i <= dbOrders_count; i++)
            {
                _orderDirectoryService.TryAddOrder(new Order() { Product = $"prod{i}", Author = DefaultUser.Profile });
            }

            //Act
            var responce = OrderController.GetAll();

            //Assert
            Assert.IsTrue(responce.Value.Count() == dbOrders_count);
        }

        [Test]
        public void GetOrder_ById()
        {
            //Arrange
            string prod = "some car";
            _orderDirectoryService.TryAddOrder(new Order() { Product = prod, Author = DefaultUser.Profile });
            int orderId = _orderDirectoryService.GetAllOrders().Single().Id;

            //Act
            var responce = OrderController.Get(orderId);

            //Assert
            Assert.IsTrue(responce.Value.Product == prod);
        }

        [Test]
        public void Deny_GetOrder_By_Incorrect_Id()
        {
            //Arrange
            string prod = "some car";
            _orderDirectoryService.TryAddOrder(new Order() { Product = prod, Author = DefaultUser.Profile });
            int orderId = _orderDirectoryService.GetAllOrders().Single().Id;

            //Act
            var responce = OrderController.Get(9);

            //Assert
            Assert.IsNull(responce.Value);
            Assert.IsInstanceOf(typeof(NotFoundResult), responce.Result, $"Result type should be NotFoundResult, by current is {0}", responce.Result.GetType());
        }

        [Test]
        public void Deny_Put_New_Order()
        {
            //Act
            var responce = OrderController.Put(new Order() { Product = "prod", AuthorId = DefaultUser.Profile.Id });

            //Assert
            Assert.IsNull(responce.Value);
            Assert.IsInstanceOf(typeof(BadRequestResult), responce.Result, $"Result type should be BadRequestResult, by current is {0}", responce.Result.GetType());
        }

        [Test]
        public void Put_OrderProduct()
        {
            //Arrange
            _orderDirectoryService.TryAddOrder(new Order() { Product = "Apple", AuthorId = DefaultUser.Profile.Id });
            _orderDirectoryService.TryAddOrder(new Order() { Product = "prod_2", AuthorId = DefaultUser.Profile.Id });
            var order = _orderDirectoryService.GetAllOrders().First();

            //Act
            string newProduct = "Pinapple";
            order.Product = newProduct;
            var responce = OrderController.Put(order);

            //Assert
            Assert.IsTrue(responce.Value.Product == newProduct);
            Assert.IsTrue(_orderDirectoryService.GetAllOrders().First().Product == newProduct);
        }

        [Test]
        public void Deny_PutOrder_with_incorrect_AuthorId()
        {
            //Arrange
            _orderDirectoryService.TryAddOrder(new Order() { Product = "test1", AuthorId =  DefaultUser.Profile.Id});
            var order = _orderDirectoryService.GetAllOrders()[0];

            //Act
            order.AuthorId = 8;
            var responce = OrderController.Put(order);

            //Assert
            Assert.IsNull(responce.Value);
            Assert.IsInstanceOf(typeof(BadRequestResult), responce.Result, $"Result type should be BadRequestResult, by current is {0}", responce.Result.GetType());
        }

        //TODO add test to deny put order with new author
    }
}
