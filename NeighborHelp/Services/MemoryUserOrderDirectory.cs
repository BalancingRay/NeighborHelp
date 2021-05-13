using NeighborHelpModels.Models;
using NeighborHelpModels.Models.Consts;
using NeighborHelpInfrastructure.ServiceContracts;
using System.Collections.Generic;
using System.Linq;
using NeighborHelpModels.Extentions;

namespace NeighborHelp.Services
{
    public class MemoryUserOrderDirectory : IUserDirectoryServise, IOrderDirectoryServise
    {
        private List<User> Users;
        private List<Order> Orders;

        public MemoryUserOrderDirectory()
        {
            Users = new List<User>();
            Orders = new List<Order>();
        }

        #region IOrderDirectoryServise implementation

        public bool TryAddOrder(Order order)
        {
            if (order == null)
                return false;

            if (order.AuthorId < 1)
            {
                order.AuthorId = order.Author?.Id ?? -1;
            }
            order.Author = GetUser(order.AuthorId)?.Profile;

            bool isProductEmpty = string.IsNullOrWhiteSpace(order?.Product);

            if (order.Author == null || isProductEmpty)
            {
                return false;
            }
            else
            {
                if (string.IsNullOrEmpty(order.Status))
                {
                    order.Status = OrderStatus.INITIALIZE;
                }

                int lastID = Orders.LastOrDefault()?.Id ?? 0;
                order.Id = ++lastID;
                Orders.Add(order);//.Dublicate());

                return true;
            }
        }

        public Order GetOrder(int id, bool useTracking = false)
        {
            var order = Orders.FirstOrDefault(cl => cl.Id == id);
            return useTracking ? order : order.Dublicate();
        }

        public IList<Order> GetOrders(int userId, bool useTracking = false)
        {
            var orders = Orders.Where(cl => cl.Author.Id == userId).ToList();
            return useTracking ? orders : orders.Dublicate();
        }

        public IList<Order> GetAllOrders(bool useTracking = false)
        {
            var orders = Orders.ToList();
            return useTracking ? orders : orders.Dublicate();
        }

        public bool TryPutOrder(Order order)
        {
            var oldOrder = Orders.FirstOrDefault(cl => (cl.Id == order.Id
                                              && cl.AuthorId == order?.AuthorId));

            if (oldOrder != null)
            {
                Orders[Orders.IndexOf(oldOrder)] = order;//.Dublicate();

                return true;
            }

            return false;
        }

        public bool TryRemoveOrder(int id)
        {
            var order = Orders.FirstOrDefault(o => o.Id == id);

            if (order == null)
                return false;

            Orders.Remove(order);
            return true;
        }

        #endregion IOrderDirectoryServise implementation

        #region IUserDirectoryServise implementation

        public bool TryAddUser(User user)
        {
            if (user == null)
                return false;

            bool isLoginEmpty = string.IsNullOrWhiteSpace(user.Login);
            bool isLoginExist = Users.Any(u => u.Login == user.Login);
            bool isProfileDublicated = user.Profile != null && Users.Any(u => u.Profile == user.Profile);

            if (isLoginEmpty || isLoginExist || isProfileDublicated)
            {
                return false;
            }
            else
            {
                int lastID = Users.LastOrDefault()?.Id ?? 0;
                user.Id = ++lastID;
                if (user.Profile != null)
                {
                    //int lastProfileId = Users.Where(u => user.Profile != null).LastOrDefault()?.Id ?? 0;
                    user.Profile.Id = user.Id;
                }
                Users.Add(user);//.Dublicate());

                return true;
            }
        }

        public User GetUser(int id, bool useTracking = false)
        {
            var user = Users.FirstOrDefault(u => u.Id == id);

            return useTracking ? user : user.Dublicate();
        }

        public User GetUser(string login, string password)
        {
            var user = Users.FirstOrDefault(u => u.Login == login && u.Password == password);

            return user.Dublicate();
        }

        public IList<User> GetUsers(bool useTracking = false)
        {
            var users = Users.ToList();
            return useTracking ? users : users.Dublicate();
        }

        public bool TryPutUser(User user)
        {
            var oldUser = Users.FirstOrDefault(u => (u.Login == user?.Login
                                               && u.Id == user?.Id));

            if (oldUser != null)
            {
                Users[Users.IndexOf(oldUser)] = user;//.Dublicate();

                return true;
            }

            return false;
        }

        public bool TryRemoveUser(int id, bool removeRelatedOrders)
        {
            if (!Users.Any(u => u.Id == id))
                return false;

            if (removeRelatedOrders)
            {
                Orders.RemoveAll(o => o.AuthorId == id);
            }
            else if (Orders.Any(o => o.AuthorId == id))
            {
                return false;
            }

            Users.RemoveAll(u => u.Id == id);

            return true;
        }

        #endregion IUserDirectoryServise
    }
}
