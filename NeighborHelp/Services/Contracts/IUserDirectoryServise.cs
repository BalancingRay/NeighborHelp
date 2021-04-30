using NeighborHelp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeighborHelp.Services.Contracts
{
    public interface IUserDirectoryServise
    {
        public bool TryAddUser(User user);
        public User GetUser(int id, bool useTracking = false);
        public User GetUser(string login, string password);
        public IList<User> GetUsers(bool useTracking = false);
        public bool TryPutUser(User user);
    }
}
