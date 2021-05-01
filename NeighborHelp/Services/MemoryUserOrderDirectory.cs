using NeighborHelp.Models;
using NeighborHelp.Models.Consts;
using NeighborHelp.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            bool isAuthorNotInitialized = order == null
                || (string.IsNullOrWhiteSpace(order.Author?.Name) && order.AuthorId == 0);

            bool isProductEmpty = string.IsNullOrWhiteSpace(order?.Product);

            if (isAuthorNotInitialized || isProductEmpty)
            {
                return false;
            }
            else
            {
                if (string.IsNullOrEmpty(order.Status))
                {
                    order.Status = OrderStatus.INITIALIZE;
                }

                if(order.Author != null)
                {
                    order.AuthorId = order.Author.Id;
                }
                else
                {
                    order.Author = GetUser(order.AuthorId)?.Profile;
                }

                int lastID = Orders.LastOrDefault()?.Id ?? 0;
                order.Id = ++lastID;
                Orders.Add(order.Dublicate());

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
                Orders[Orders.IndexOf(oldOrder)] = order.Dublicate();

                return true;
            }

            return false;
        }

        #endregion IOrderDirectoryServise implementation

        #region IUserDirectoryServise implementation

        public bool TryAddUser(User user)
        {
            bool isLoginEmpty = string.IsNullOrWhiteSpace(user?.Login);
            bool isLoginExist = Users.Any(u => u.Login == user?.Login);

            if (isLoginEmpty || isLoginExist)
            {
                return false;
            }
            else
            {
                int lastID = Users.LastOrDefault()?.Id ?? 0;
                user.Id = ++lastID;
                if (user.Profile != null)
                {
                    user.Profile.Id = user.Id;
                }
                Users.Add(user.Dublicate());

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
                Users[Users.IndexOf(oldUser)] = user.Dublicate();

                return true;
            }

            return false;
        }

        #endregion IUserDirectoryServise
    }
}
