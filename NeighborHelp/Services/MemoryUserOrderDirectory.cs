using NeighborHelp.Models;
using NeighborHelp.Models.Consts;
using NeighborHelp.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeighborHelp.Services
{
    public class MemoryUserOrderDirectory: IUserDirectoryServise, IOrderDirectoryServise
    {
        private List<User> Users;
        private List<Order> Orders;

        public MemoryUserOrderDirectory()
        {
            Users = new List<User>();
            Orders = new List<Order>();

            FillUsers();
            FillOrders(Users.FirstOrDefault());
            FillOrders2(Users.LastOrDefault());
        }

        private void FillUsers()
        {
            var adminProfile = new UserProfile() { Name = "Vasil", Address = "Minsk" };
            var user1Profile = new UserProfile() { Name = "Ilia", Address = "Krasnaya str. 28", PhoneNumber = "299-87-65" };

            var admin = new User() { Login = "admin", Password = "admin", Role = UserRoles.ADMIN, 
                UserName = "SuperAdmin", Profile= adminProfile };
            var user = new User() { Login = "user1", Password = "1234", Role = UserRoles.USER, 
                UserName = "TestUser", Profile = user1Profile };

            TryAddUser(admin);
            TryAddUser(user);
        }

        private void FillOrders(User user)
        {
            var userOrder = new Order()
            {
                Author = user.Profile,
                AuthorID = user.Id,
                OrderType = OrderTypes.SELL,
                Product = "Диван, б/у",
                ProductDescription = "Коричневый, мягкая обивка, состояние 7 из 10",
                Cost = 50
            };

            var userOrder2 = new Order()
            {
                Author = user.Profile,
                AuthorID = user.Id,
                OrderType = OrderTypes.BUY,
                Product = "Грамофон",
                ProductDescription = "Рабочий. Для проигрывания виниловых пластинок, можно полностью механический",
                Cost = 80
            };

            TryAddOrder(userOrder);
            TryAddOrder(userOrder2);
        }

        private void FillOrders2(User user)
        {
            var userOrder = new Order()
            {
                Author = user.Profile,
                AuthorID = user.Id,
                OrderType = OrderTypes.SELL,
                Product = "Капуста",
                ProductDescription = "Среднего размера. Хорошее качество. Для засолки",
                Cost = 3
            };

            var userOrder2 = new Order()
            {
                Author = user.Profile,
                AuthorID = user.Id,
                OrderType = OrderTypes.BUY,
                Product = "3 литровые банки",
                ProductDescription = "Стреклянные банки 3 литра. б/у. Без сколов и трещин. Под закатки",
                Cost = 3
            };

            TryAddOrder(userOrder);
            TryAddOrder(userOrder2);
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
                                              && cl.AuthorID == order?.AuthorID));

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
            bool isUserNameEmpty = string.IsNullOrWhiteSpace(user?.Login);
            bool isUserExist = Users.Any(u => u.Login == user?.Login);

            if (isUserNameEmpty || isUserExist)
            {
                return false;
            }
            else
            {
                int lastID = Users.LastOrDefault()?.Id ?? 0;
                user.Id = ++lastID;
                if (user.Profile !=null)
                {
                    user.Profile.Id = user.Id;
                }
                Users.Add(user);

                return true;
            }
        }

        public User GetUser(int id)
        {
            var user = Users.FirstOrDefault(u => u.Id == id);

            return user;
        }

        public User GetUser(string login, string password)
        {
            return Users.FirstOrDefault(u => u.Login == login && u.Password == password);
        }

        public IList<User> GetUsers()
        {
            return Users.ToList();
        }

        public bool TryPutUser(User user)
        {
            var oldUser = Users.FirstOrDefault( u => (u.Login == user?.Login 
                                                && u.Id == user?.Id));

            if (oldUser !=null)
            {
                Users[Users.IndexOf(oldUser)] = user;

                return true;
            }

            return false;
        }

        #endregion IUserDirectoryServise
    }
}
