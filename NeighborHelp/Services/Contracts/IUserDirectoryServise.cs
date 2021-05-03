using NeighborHelpModels.Models;
using System.Collections.Generic;

namespace NeighborHelp.Services.Contracts
{
    public interface IUserDirectoryServise
    {
        public bool TryAddUser(User user);
        public User GetUser(int id, bool useTracking = ContractConsts.DefaultTracking);
        public User GetUser(string login, string password);
        public IList<User> GetUsers(bool useTracking = ContractConsts.DefaultTracking);
        public bool TryPutUser(User user);

        public bool TryRemoveUser(int id, bool removeRelatedOrders);
    }
}
