using NeighborHelp.Models;
using NeighborHelp.Services;
using NeighborHelp.Services.Contracts;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeighborHelpTests.Tests
{
    public class MemoryOrderDirectoryTest : OrderDirectoryTestBase
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
            var memoryDirectory = new MemoryUserOrderDirectory();
            _userDirectoryInMemory = memoryDirectory;
            _orderDirectoryInMemory = memoryDirectory;

            _userProfile1 = CreateDefaultUserProfile();
            _userProfile2 = CreateDefaultUserProfile(
                login: "user2", profileName: "Ivan", 
                address: "Prityckova str.29", phoneNumber: "123-45-678");
        }
    }
}
