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
