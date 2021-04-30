using Microsoft.EntityFrameworkCore;
using NeighborHelp.Models;
using NeighborHelp.Models.Consts;
using NeighborHelp.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeighborHelp.Services
{
    public class EntityUserOrderDirectory : IOrderDirectoryServise, IUserDirectoryServise
    {
        protected ApplicationContext DataBase { get;}

        protected DbSet<User> Users => DataBase.Users;
        protected DbSet<Order> Orders => DataBase.Orders;

        public EntityUserOrderDirectory(ApplicationContext context)
        {
            DataBase = context;
        }

        #region IUserDirectoryServise implementation

        public bool TryAddUser(User user)
        {
            if (user == null)
                return false;

            bool isLoginEmpty = string.IsNullOrWhiteSpace(user.Login);
            bool isLoginExist = Users.Any(u => u.Login == user.Login);

            if (isLoginEmpty || isLoginExist)
            {
                return false;
            }
            else
            {
                Users.Add(user);
                DataBase.SaveChanges();

                return true;
            }
        }

        public User GetUser(int id, bool useTraching = false)
        {
            var user = useTraching ? Users.SingleOrDefault(u => u.Id == id) : Users.AsNoTracking().Include(u => u.Profile).SingleOrDefault(u => u.Id == id);
            return user;
        }

        public User GetUser(string login, string password)
        {
            var user = Users.AsNoTracking().SingleOrDefault(u => u.Login == login && u.Password == password);

            return user;
        }

        public IList<User> GetUsers(bool useTraching = false)
        {
            return useTraching ? Users.ToList() : Users.AsNoTracking().Include(u => u.Profile).ToList();
        }

        public bool TryPutUser(User user)
        {
            if (Users.Any(u => u.Login == user.Login && u.Id == user.Id))
            {
                bool isDetached = DataBase.Entry(user).State == EntityState.Detached;

                if (isDetached)
                {
                    Users.Update(user);
                }
                
                DataBase.SaveChanges();

                return true;
            }

            return false;
        }

        #endregion IUserDirectoryServise


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
                Orders.Add(order);
                DataBase.SaveChanges();
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
            if (order == null)
                return false;

            if (Orders.Any(cl => (cl.ID == order.ID && cl.AuthorId == order.AuthorId)))
            {
                Orders.Update(order);
                DataBase.SaveChanges();

                return true;
            }

            return false;
        }

        #endregion IOrderDirectoryServise implementation
    }
}
