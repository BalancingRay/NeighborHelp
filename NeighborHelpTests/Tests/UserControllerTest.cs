using Microsoft.AspNetCore.Mvc;
using NeighborHelp.Controllers;
using NeighborHelpModels.Models;
using NeighborHelp.Services;
using NeighborHelpInfrastructure.ServiceContracts;
using NeighborHelpTests.Infrostructure;
using NUnit.Framework;
using System;
using System.Linq;

namespace NeighborHelpTests.Tests
{
    public class UserControllerTest
    {
        private bool useSQLServer = true;
        private bool useMockDirectory = false;

        private UserController _userController;
        private UserController UserController => _userController;

        private IUserDirectoryServise _directoryService;

        [SetUp]
        public void Setup()
        {
            if(useMockDirectory)
            {
                _directoryService = null;
                //TODO Initialize mock directory
                throw new NotImplementedException("MockDirectory have not implemented yet");
            }
            else if (useSQLServer)
            {
                var context = DataBaseBuilder.BuildDataBaseContext();
                _directoryService = new EntityUserOrderDirectory(context);
            }
            else
            {
                _directoryService = new MemoryUserOrderDirectory();
            }
            _userController = new UserController(_directoryService);
        }

        [Test]
        public void PostUser()
        {
            //Arrange
            var login = "someUser";
            string password = "12345";

            //Act
            var responce = UserController.Post(new User() { Login = login, Password = password });

            //Assert
            var user = _directoryService.GetUsers().Single();
            Assert.IsTrue(user.Login == login);
            Assert.IsTrue(user.Password == password);
        }

        [Test]
        public void Deny_Post_Existed_User()
        {
            //Arrange
            var user = new User() { Login = "user" };
            _directoryService.TryAddUser(user);

            //Act
            var responce = UserController.Post(user);

            //Assert
            Assert.IsNull(responce.Value);
            Assert.IsInstanceOf(typeof(BadRequestResult), responce.Result, $"Result type should be BadRequestResult, by current is {0}", responce.Result.GetType());
        }

        [TestCase(0)]
        [TestCase(2)]
        [TestCase(10)]
        public void GetAllUsers(int dbUsers_count)
        {
            //Arrange
            for(int i=1; i<=dbUsers_count; i++)
            {
                _directoryService.TryAddUser(new User() { Login = $"user_{i}" });
            }

            //Act
            var responce = UserController.GetAll();

            //Assert
            Assert.IsTrue(responce.Value.Count() == dbUsers_count);
        }

        [Test]
        public void GetUser_ById()
        {
            //Arrange
            var login = "someUser";
            _directoryService.TryAddUser(new User() { Login = login});
            int userId = _directoryService.GetUsers().Single().Id;

            //Act
            var responce = UserController.Get(userId);

            //Assert
            Assert.IsTrue(responce.Value.Login == login);
        }

        [Test]
        public void Deny_GetUser_By_Incorrect_Id()
        {
            //Arrange
            var login = "someUser";
            _directoryService.TryAddUser(new User() { Login = login });

            //Act
            var responce = UserController.Get(100);

            //Assert
            Assert.IsNull(responce.Value);
            Assert.IsInstanceOf(typeof(NotFoundResult), responce.Result, $"Result type should be NotFoundResult, by current is {0}", responce.Result.GetType());
        }

        [Test]
        public void Deny_CurentUser_without_Authentification()
        {
            //Act
            var responce = UserController.Current();

            //Assert
            Assert.IsNull(responce.Value);
            Assert.IsInstanceOf(typeof(NotFoundResult), responce.Result, $"Result type should be NotFoundResult, by current is {0}", responce.Result.GetType());
        }

        [Test]
        public void CurentUser_after_Authentification()
        {
            //Arrange
            _directoryService.TryAddUser(new User() { Login = "User_1", Role = "user" });
            var user = _directoryService.GetUsers().Single();
            UserController.ControllerContext = ControllerContextAuthentificator.Authentificate(user);

            //Act
            var responce = UserController.Current();

            //Assert
            Assert.IsTrue(responce.Value.Login == user.Login);
        }

        [Test]
        public void Deny_Put_New_User()
        {
            //Act
            var responce = UserController.Put(new User() { Login = "user", Id=2 });

            //Assert
            Assert.IsNull(responce.Value);
            Assert.IsInstanceOf(typeof(BadRequestResult), responce.Result, $"Result type should be BadRequestResult, by current is {0}", responce.Result.GetType());
        }

        [Test]
        public void Put_UserName()
        {
            //Arrange
            _directoryService.TryAddUser(new User() { Login = "Target_User" });
            _directoryService.TryAddUser(new User() { Login = "User2" });
            var user = _directoryService.GetUsers().First();

            //Act
            string newUserName = "SuperUser";
            user.UserName = newUserName;
            var responce = UserController.Put(user);

            //Assert
            Assert.IsTrue(responce.Value.UserName == newUserName);
            Assert.IsTrue(_directoryService.GetUsers().First().UserName == newUserName);
        }

        [Test]
        public void Put_UserProfile()
        {
            //Arrange
            _directoryService.TryAddUser(new User() { Login = "test1" });
            _directoryService.TryAddUser(new User() { Login = "Target_User" });
            _directoryService.TryAddUser(new User() { Login = "User3" });
            var user = _directoryService.GetUsers()[1];

            //Act
            string profileName = "Sebastyan";
            user.Profile = new UserProfile() { Name = profileName, Address = "Main str.1", PhoneNumber = "1-234-56" }; ;
            var responce = UserController.Put(user);

            //Assert
            Assert.IsTrue(responce.Value.Profile.Name == profileName);
            Assert.IsTrue(_directoryService.GetUsers()[1].Profile.Name == profileName);
        }
    }
}
