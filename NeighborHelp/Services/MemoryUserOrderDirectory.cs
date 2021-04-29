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
        private List<Role> Roles;
        private List<Order> Orders;

        public MemoryUserOrderDirectory()
        {
            Users = new List<User>();
            Orders = new List<Order>();
            Roles = new List<Role>();

            FillUsers();
            FillOrders(Users.FirstOrDefault());
        }

        private void FillUsers()
        {
            var userRole = new Role() { Name = UserRoles.USER, ID=0 };
            var adminRole = new Role() { Name = UserRoles.ADMIN, ID=1 };

            Roles.Add(userRole);
            Roles.Add(adminRole);

            var admin = new User() { Login = "admin", Password = "admin", Role = adminRole, RoleId = adminRole.ID, UserName = "Admin" };
            var user = new User() { Login = "user1", Password = "1234", Role = userRole, RoleId = userRole.ID, UserName = "TestUser" };

            TryAddUser(admin);
            TryAddUser(user);
        }

        private void FillOrders(User user)
        {
            var userOrder = new Order()
            {
                Author = user,
                AuthorID = user.Id,
                OrderType = OrderTypes.SELL,
                Product = "Диван, б/у",
                ProductDescription = "Коричневый, мягкая обивка, состояние 7 из 10",
                Cost = 50
            };

            TryAddOrder(userOrder);
        }

        #region IOrderDirectoryServise implementation

        public bool TryAddOrder(Order order)
        {
            bool isOrderNotInitialized = 
                string.IsNullOrWhiteSpace(order?.Author?.Login) 
                || string.IsNullOrWhiteSpace(order?.Product);

            if (isOrderNotInitialized)
            {
                return false;
            }
            else
            {
                order.Status = OrderStatus.INITIALIZE;
                int lastID = Orders.Last()?.ID ?? 0;
                order.ID = lastID++;
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
                int lastID = Users.Last()?.Id ?? 0;
                user.Id = lastID++;
                Users.Add(user);
                UpdateRoles(user);

                return true;
            }
        }

        private void UpdateRoles(User user)
        {
            var role = Roles.FirstOrDefault(r => r.Name == user.Role?.Name);

            if (role != null 
                && !role.Users.Any(u => u.Id == user.Id))
            {
                role.Users.Add(user);
            }
        }

        public User GetUser(int id)
        {
            return Users.FirstOrDefault(u => u.Id == id);
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
