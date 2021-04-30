using NeighborHelp.Services;
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
        [SetUp]
        public void Setup()
        {
            UserDirectory = new MemoryUserOrderDirectory();
        }
    }
}
