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
        protected ApplicationContext DataBase { get; }

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
            if (useTraching)
            {
                return Users.SingleOrDefault(u => u.Id == id);
            }
            else
            {
                return Users.AsNoTracking().Include(u => u.Profile).SingleOrDefault(u => u.Id == id);
            }
        }

        public User GetUser(string login, string password)
        {
            var user = Users.AsNoTracking().SingleOrDefault(u => u.Login == login && u.Password == password);

            return user;
        }

        public IList<User> GetUsers(bool useTraching = false)
        {
            if (useTraching)
            {
                return Users.ToList();
            }
            else
            {
                return Users.AsNoTracking().Include(u => u.Profile).ToList();
            }
        }

        public bool TryPutUser(User user)
        {
            if (Users.Any(u => u.Login == user.Login && u.Id == user.Id))
            {
                if (IsDetached(user))
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
                Orders.Add(order);
                DataBase.SaveChanges();
                return true;
            }
        }

        public Order GetOrder(int id, bool useTracking = false)
        {
            if (useTracking)
            {
                return Orders.SingleOrDefault(cl => cl.Id == id);
            }
            else
            {
                return Orders.AsNoTracking().Include(o => o.Author).SingleOrDefault(cl => cl.Id == id);
            }
        }

        public IList<Order> GetOrders(int userId, bool useTracking = false)
        {
            if (useTracking)
            {
                return Orders.Where(cl => cl.Author.Id == userId).ToList();
            }
            else
            {
                return Orders.AsNoTracking().Include(o => o.Author).Where(cl => cl.Author.Id == userId).ToList();
            }

        }

        public IList<Order> GetAllOrders(bool useTracking = false)
        {
            if (useTracking)
            {
                return Orders.ToList();
            }
            else
            {
                return Orders.AsNoTracking().Include(o => o.Author).ToList();
            }

        }

        public bool TryPutOrder(Order order)
        {
            if (order == null)
                return false;

            if (Orders.Any(cl => (cl.Id == order.Id && cl.AuthorId == order.AuthorId)))
            {
                if (IsDetached(order))
                {
                    Orders.Update(order);
                }

                DataBase.SaveChanges();

                return true;
            }

            return false;
        }

        #endregion IOrderDirectoryServise implementation

        private bool IsDetached<TEntity>(TEntity entity) where TEntity : class
        {
            var entityState = DataBase.Entry(entity).State;
            return entityState == EntityState.Detached;
        }
    }
}
