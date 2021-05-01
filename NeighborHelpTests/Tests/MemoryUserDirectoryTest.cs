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
        private IUserDirectoryServise _UserDirectoryInMemory;
        public override IUserDirectoryServise UserDirectory => _UserDirectoryInMemory;

        [SetUp]
        public void Setup()
        {
            _UserDirectoryInMemory = new MemoryUserOrderDirectory();
        }
    }
}
