using NeighborHelp.Models;
using NeighborHelp.Services;
using NeighborHelp.Services.Contracts;
using NUnit.Framework;
using System.Collections.Generic;

namespace NeighborHelpTests.Tests
{
    public abstract class UserDirectoryTestBase
    {
        public IUserDirectoryServise UserDirectory { get; protected set; }

        #region AddUser tests

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(10)]
        public void TryAddUserTest(int countOfUser)
        {
            bool result = true;

            for(int i =1; i <= countOfUser; i++)
            {
                result = UserDirectory.TryAddUser(new User() { Login = $"user{i}" });
            }

            var users = UserDirectory.GetUsers();

            Assert.IsTrue(result == true && users.Count == countOfUser);
        }

        [Test]
        public void UniqueUserIdTest()
        {
            UserDirectory.TryAddUser(new User() { Login = "user1" });
            UserDirectory.TryAddUser(new User() { Login = "user2" });

            var users = UserDirectory.GetUsers();

            Assert.IsTrue(users[0].Id != users[1].Id);
        }

        [Test]
        public void DenyAddDublicateUserTest()
        {
            var user = new User() { Login = "user1" };

            UserDirectory.TryAddUser(user);
            UserDirectory.TryAddUser(user);

            var users = UserDirectory.GetUsers();

            Assert.IsTrue(users.Count == 1);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public void DenyInvalidOrEmptyLoginTest(string login)
        {
            bool result = UserDirectory.TryAddUser(new User() { Login = login });

            Assert.IsTrue(result == false && UserDirectory.GetUsers().Count == 0);
        }

        [Test]
        public void DenyAddNullUserTest()
        {
            bool result = UserDirectory.TryAddUser(null);

            Assert.IsTrue(result == false && UserDirectory.GetUsers().Count == 0);
        }

        #endregion AddUser tests


        //TODO add tests to check tracking option
        #region GetUser tests
        [Test]
        public void GetUserLoginTest()
        {
            string login = "user1";

            UserDirectory.TryAddUser(new User() { Login = login });

            var users = UserDirectory.GetUsers();

            Assert.IsTrue(users[0].Login == login);
        }

        [Test]
        public void GetUserNameTest()
        {
            string userName = "superMan";

            UserDirectory.TryAddUser(new User() { Login = "user1", UserName = userName });

            var users = UserDirectory.GetUsers();

            Assert.IsTrue(users[0].UserName == userName);
        }

        [Test]
        public void GetRoleTest()
        {
            string role = "testRole";

            UserDirectory.TryAddUser(new User() { Login = "user1", Role = role });

            var users = UserDirectory.GetUsers();

            Assert.IsTrue(users[0].Role == role);
        }

        [Test]
        public void GetProfileNameTest()
        {
            string profileName = "Vasil";
            var profile = new UserProfile() { Name = profileName };
            UserDirectory.TryAddUser(new User() { Login = "user1", Profile = profile });

            var users = UserDirectory.GetUsers();

            Assert.IsTrue(users[0].Profile.Name == profileName);
        }

        [Test]
        public void GetProfileAddressTest()
        {
            string address = "Central park street. 25";
            var profile = new UserProfile() { Address = address };
            UserDirectory.TryAddUser(new User() { Login = "user1", Profile = profile });

            var users = UserDirectory.GetUsers();

            Assert.IsTrue(users[0].Profile.Address == address);
        }

        [Test]
        public void GetProfilePhoneNumberTest()
        {
            string phoneNumber = "123-25-678";
            var profile = new UserProfile() { PhoneNumber = phoneNumber };
            UserDirectory.TryAddUser(new User() { Login = "user1", Profile = profile });

            var users = UserDirectory.GetUsers();

            Assert.IsTrue(users[0].Profile.PhoneNumber == phoneNumber);
        }

        [Test]
        public void EqualProfileIdAndUserIdTest()
        {
            var profile = new UserProfile();
            UserDirectory.TryAddUser(new User() { Login = "user1", Profile = profile });

            var users = UserDirectory.GetUsers(true);

            Assert.IsTrue(users[0].Profile.Id == users[0].Id);
        }

        [Test]
        public void GetUserByPasswordTest()
        {
            string log = "user1";
            string pass = "password12345";

            UserDirectory.TryAddUser(new User() { Login = log, Password = pass });

            var user = UserDirectory.GetUser(log, pass);

            Assert.IsTrue(user.Login == log);
        }

        [Test]
        public void GetUserByIdTest()
        {
            string log = "user1";

            UserDirectory.TryAddUser(new User() { Login = log });

            int userId = UserDirectory.GetUsers()[0].Id;
            var user = UserDirectory.GetUser(userId);

            Assert.IsTrue(user.Login == log);
        }

        #endregion GetUser Tests


        //TODO add tests to check tracking option
        #region PutUser Tests

        //TODO Why this test not working in case during thacking disabled
        [Test]
        public void PutUserNameTest()
        {
            UserDirectory.TryAddUser(new User() { Login = "user1" });
            string userName = "TestName";

            var firstUser = UserDirectory.GetUsers(true)[0];
            firstUser.UserName = userName;
            bool result = UserDirectory.TryPutUser(firstUser);

            firstUser = UserDirectory.GetUsers(true)[0];
            Assert.IsTrue(result && firstUser.UserName == userName);
        }

        //TODO Why this test not working in case during thacking disabled
        [Test]
        public void PutUserProfileTest()
        {
            UserDirectory.TryAddUser(new User() { Login = "user1" });
            string profileName = "Ilya";
            string profilePhoneNumber = "100-400-600";
            string profileAddress = "Cenral str.98";

            var firstUser = UserDirectory.GetUsers(true)[0];
            firstUser.Profile = new UserProfile()
            {
                Name = profileName,
                PhoneNumber = profilePhoneNumber,
                Address = profileAddress
            };
            bool result = UserDirectory.TryPutUser(firstUser);

            firstUser = UserDirectory.GetUsers(true)[0];
            Assert.IsTrue(result 
                && firstUser.Profile.Name == profileName
                && firstUser.Profile.Address == profileAddress
                && firstUser.Profile.PhoneNumber == profilePhoneNumber);
        }

        [Test]
        public void DenyPutNewUserTest()
        {
            bool result = UserDirectory.TryPutUser(new User() { Login = "user1" });

            var userCount = UserDirectory.GetUsers().Count;
            Assert.IsTrue(result==false && userCount == 0);
        }

        [Test]
        public void DenyChangeLoginTest()
        {
            string oldLogin = "firstUser";

            UserDirectory.TryAddUser(new User() { Login = oldLogin });

            var firstUser = UserDirectory.GetUsers()[0];
            firstUser.Login = "user20";
            bool result = UserDirectory.TryPutUser(firstUser);

            firstUser = UserDirectory.GetUsers()[0];
            Assert.IsTrue(result == false && firstUser.Login == oldLogin);
        }

        [Test]
        public void DenyChangeUserIdTest()
        {
            UserDirectory.TryAddUser(new User() { Login = "firstUser" });
            UserDirectory.TryAddUser(new User() { Login = "secondUser" });
            string newUserName = "Ivan";

            var firstUser = UserDirectory.GetUsers()[0];
            firstUser.Id++;
            firstUser.UserName = newUserName;
            bool result = UserDirectory.TryPutUser(firstUser);

            firstUser = UserDirectory.GetUsers()[0];
            Assert.IsTrue(result == false && firstUser.UserName != newUserName);
        }

        #endregion PutUser tests
    }
}