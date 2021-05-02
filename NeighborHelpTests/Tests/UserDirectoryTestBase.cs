using NeighborHelp.Models;
using NeighborHelp.Services.Contracts;
using NUnit.Framework;
using System.Linq;

namespace NeighborHelpTests.Tests
{
    public abstract class UserDirectoryTestBase
    {
        public abstract IUserDirectoryServise UserDirectory { get; }

        #region AddUser tests

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(10)]
        public void AddUser(int countOfUser)
        {
            bool result = true;

            for (int i = 1; i <= countOfUser; i++)
            {
                result = UserDirectory.TryAddUser(new User() { Login = $"user{i}" });
            }

            var users = UserDirectory.GetUsers();

            Assert.IsTrue(result == true && users.Count == countOfUser);
        }

        [Test]
        public void Unique_UserId()
        {
            UserDirectory.TryAddUser(new User() { Login = "user1" });
            UserDirectory.TryAddUser(new User() { Login = "user2" });

            var users = UserDirectory.GetUsers();

            Assert.IsTrue(users[0].Id != users[1].Id);
        }

        [Test]
        public void Deny_AddDublicateUser()
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
        public void Deny_InvalidOrEmptyLogin(string login)
        {
            bool result = UserDirectory.TryAddUser(new User() { Login = login });

            Assert.IsTrue(result == false && UserDirectory.GetUsers().Count == 0);
        }

        [Test]
        public void DenyAddNullUser()
        {
            bool result = UserDirectory.TryAddUser(null);

            Assert.IsTrue(result == false && UserDirectory.GetUsers().Count == 0);
        }

        #endregion AddUser tests


        #region GetUser tests
        [Test]
        public void GetUserLogin()
        {
            string login = "user1";

            UserDirectory.TryAddUser(new User() { Login = login });

            var users = UserDirectory.GetUsers();

            Assert.IsTrue(users[0].Login == login);
        }

        [Test]
        public void GetUserName()
        {
            string userName = "superMan";

            UserDirectory.TryAddUser(new User() { Login = "user1", UserName = userName });

            var users = UserDirectory.GetUsers();

            Assert.IsTrue(users[0].UserName == userName);
        }

        [Test]
        public void GetRole()
        {
            string role = "testRole";

            UserDirectory.TryAddUser(new User() { Login = "user1", Role = role });

            var users = UserDirectory.GetUsers();

            Assert.IsTrue(users[0].Role == role);
        }

        [Test]
        public void GetProfileName()
        {
            string profileName = "Vasil";
            var profile = new UserProfile() { Name = profileName };
            UserDirectory.TryAddUser(new User() { Login = "user1", Profile = profile });

            var users = UserDirectory.GetUsers();

            Assert.IsTrue(users[0].Profile.Name == profileName);
        }

        [Test]
        public void GetProfileAddress()
        {
            string address = "Central park street. 25";
            var profile = new UserProfile() { Address = address };
            UserDirectory.TryAddUser(new User() { Login = "user1", Profile = profile });

            var users = UserDirectory.GetUsers();

            Assert.IsTrue(users[0].Profile.Address == address);
        }

        [Test]
        public void GetProfilePhoneNumber()
        {
            string phoneNumber = "123-25-678";
            var profile = new UserProfile() { PhoneNumber = phoneNumber };
            UserDirectory.TryAddUser(new User() { Login = "user1", Profile = profile });

            var users = UserDirectory.GetUsers();

            Assert.IsTrue(users[0].Profile.PhoneNumber == phoneNumber);
        }

        [Test]
        public void GetUser_ByPassword()
        {
            string log = "user1";
            string pass = "password12345";

            UserDirectory.TryAddUser(new User() { Login = log, Password = pass });

            var user = UserDirectory.GetUser(log, pass);

            Assert.IsTrue(user.Login == log);
        }

        [Test]
        public void GetUser_ById()
        {
            string log = "user1";

            UserDirectory.TryAddUser(new User() { Login = log });

            int userId = UserDirectory.GetUsers()[0].Id;
            var user = UserDirectory.GetUser(userId);

            Assert.IsTrue(user.Login == log);
        }

        #endregion GetUser Tests


        #region Tracking tests

        [Test]
        public void Get_tracked_ChangingUser()
        {
            bool trackingOption = true;
            string originalName = "Spider";
            string newName = "Gorilla";

            UserDirectory.TryAddUser(new User() { UserName = originalName, Login = "user1" });

            UserDirectory.GetUsers(trackingOption).Single()
                .UserName = newName;
            var user = UserDirectory.GetUsers(trackingOption).Single();

            Assert.IsTrue(user.UserName == newName);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Get_ChangingUser_trackingOption(bool tracking)
        {
            //Arrange
            string originalName = "Spider";
            UserDirectory.TryAddUser(new User() { UserName = originalName, Login = "user1" });

            //Act
            string newName = "Gorilla";
            UserDirectory.GetUsers(tracking).Single().UserName = newName;

            var user = UserDirectory.GetUsers(tracking).Single();

            //Assert
            if (tracking)
            {
                Assert.IsTrue(user.UserName == newName);
            }
            else
            {
                Assert.IsTrue(user.UserName == originalName);
            }
        }

        #endregion Tracking tests


        #region PutUser Tests

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void Equal_ProfileId_And_UserId(int userNumber)
        {
            //Act
            UserDirectory.TryAddUser(new User() { Login = "user1" });
            UserDirectory.TryAddUser(new User() { Login = "user2", Profile = new UserProfile() });
            UserDirectory.TryAddUser(new User() { Login = "user3" });
            UserDirectory.TryAddUser(new User() { Login = "user4", Profile = new UserProfile() });

            //Assert
            var user = UserDirectory.GetUsers()[userNumber];
            if (user.Profile == null)
            {
                Assert.IsFalse(UserDirectory.GetUsers()
                    .Any(u => u.Profile != null && u.Profile.Id == user.Id));
            }
            else
            {
                Assert.IsTrue(user.Profile.Id == user.Id);
            }
        }

        [Test]
        public void Deny_AddUser_with_DublicatedProfile()
        {
            var profile = new UserProfile();
            UserDirectory.TryAddUser(new User() { Login = "user1", Profile = profile });
            bool result = UserDirectory.TryAddUser(new User() { Login = "user2", Profile = profile });

            Assert.IsFalse(result);
            Assert.IsFalse(UserDirectory.GetUsers().Count() == 2);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void PutUserName_trackingOption(bool useTracking)
        {
            UserDirectory.TryAddUser(new User() { Login = "user1" });
            string userName = "TestName";

            var firstUser = UserDirectory.GetUsers(useTracking).Single();
            firstUser.UserName = userName;
            bool result = UserDirectory.TryPutUser(firstUser);

            firstUser = UserDirectory.GetUsers(useTracking).Single();
            Assert.IsTrue(result && firstUser.UserName == userName);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void PutUserProfile_trackingOption(bool useTracking)
        {
            UserDirectory.TryAddUser(new User() { Login = "user1" });
            string profileName = "Ilya";
            string profilePhoneNumber = "100-400-600";
            string profileAddress = "Cenral str.98";

            var firstUser = UserDirectory.GetUsers(useTracking).Single();
            firstUser.Profile = new UserProfile()
            {
                Name = profileName,
                PhoneNumber = profilePhoneNumber,
                Address = profileAddress
            };
            bool result = UserDirectory.TryPutUser(firstUser);

            firstUser = UserDirectory.GetUsers(useTracking).Single();
            Assert.IsTrue(result
                && firstUser.Profile.Name == profileName
                && firstUser.Profile.Address == profileAddress
                && firstUser.Profile.PhoneNumber == profilePhoneNumber);
        }

        public void Deny_PutNewUser()
        {
            bool result = UserDirectory.TryPutUser(new User() { Login = "user1" });

            var userCount = UserDirectory.GetUsers().Count;
            Assert.IsTrue(result == false && userCount == 0);
        }

        //Test works only when tracking disable.
        //In case when tracking enable, DBSet will be update immediately with changing entity.
        [Test]
        public void Deny_ChangeLogin()
        {
            bool useTracking = false;
            string oldLogin = "firstUser";

            UserDirectory.TryAddUser(new User() { Login = oldLogin });

            var firstUser = UserDirectory.GetUsers(useTracking)[0];
            firstUser.Login = "user20";
            bool result = UserDirectory.TryPutUser(firstUser);

            firstUser = UserDirectory.GetUsers()[0];
            Assert.IsTrue(result == false && firstUser.Login == oldLogin);
        }

        //Test works only when tracking disable.
        //In case when tracking enable, DBSet will be update immediately with changing entity.
        [Test]
        public void Deny_ChangeUserId()
        {
            bool useTracking = false;
            UserDirectory.TryAddUser(new User() { Login = "firstUser" });
            UserDirectory.TryAddUser(new User() { Login = "secondUser" });
            string newUserName = "Ivan";

            var firstUser = UserDirectory.GetUsers(useTracking)[0];
            firstUser.Id++;
            firstUser.UserName = newUserName;
            bool result = UserDirectory.TryPutUser(firstUser);

            firstUser = UserDirectory.GetUsers()[0];
            Assert.IsTrue(result == false && firstUser.UserName != newUserName);
        }

        #endregion PutUser tests
    }
}