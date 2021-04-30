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
            bool isOrderNotInitialized =
                string.IsNullOrWhiteSpace(order?.Author?.Name)
                || string.IsNullOrWhiteSpace(order?.Product);

            if (isOrderNotInitialized)
            {
                return false;
            }
            else
            {
                order.Status = OrderStatus.INITIALIZE;
                int lastID = Orders.LastOrDefault()?.ID ?? 0;
                order.ID = ++lastID;
                Orders.Add(order);

                return true;
            }
        }

        public Order GetOrder(int id)
        {
            return Orders.FirstOrDefault(cl => cl.ID == id);
        }

        public IList<Order> GetOrders(int userId)
        {
            return Orders.Where(cl => cl.Author.Id == userId).ToList();
        }

        public IList<Order> GetAllOrders()
        {
            return Orders.ToList();
        }

        public bool TryPutOrder(Order order)
        {
            var oldOrder = Orders.FirstOrDefault(cl => (cl.ID == order.ID
                                              && cl.AuthorId == order?.AuthorId));

            if (oldOrder != null)
            {
                Orders[Orders.IndexOf(oldOrder)] = order;

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
