using NeighborHelp.Services;
using NeighborHelp.Services.Contracts;
using NUnit.Framework;

namespace NeighborHelpTests.Tests
{
    public class MemoryUserDirectoryTest:UserDirectoryTestBase
    {
        private MemoryUserOrderDirectory _directoryInMemory;
        public override IUserDirectoryServise UserDirectory => _directoryInMemory;

        public override IOrderDirectoryServise OrderDirectory => _directoryInMemory;

        [SetUp]
        public void Setup()
        {
            _directoryInMemory = new MemoryUserOrderDirectory();
        }
    }
}
