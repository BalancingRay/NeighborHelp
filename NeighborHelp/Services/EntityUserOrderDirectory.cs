using Microsoft.EntityFrameworkCore;
using NeighborHelpModels.Models;
using NeighborHelpModels.Models.Consts;
using NeighborHelp.Services.Contracts;
using System.Collections.Generic;
using System.Linq;
using NeighborHelpModels.Utils;

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
            bool isProfileDublicated = user.Profile != null && Users.Any(u => u.Profile == user.Profile);

            if (isLoginEmpty || isLoginExist || isProfileDublicated)
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

        public User GetUser(int id, bool useTraching)
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

        public IList<User> GetUsers(bool useTraching)
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
                    var originalUser = GetUser(user.Id, true);
                    originalUser.UpdateFrom(user);
                }

                DataBase.SaveChanges();

                return true;
            }

            return false;
        }

        public bool TryRemoveUser(int id, bool removeRelatedOrders)
        {
            var user = Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
                return false;

            var orders = Orders.Where(o => o.AuthorId == id).ToArray();

            if (removeRelatedOrders)
            {
                Orders.RemoveRange(orders);
            }
            else if (orders.Count() > 0)
            {
                return false;
            }

            Users.Remove(user);

            DataBase.SaveChanges();

            return true;
        }

        #endregion IUserDirectoryServise


        #region IOrderDirectoryServise implementation

        public bool TryAddOrder(Order order)
        {
            if (order == null)
                return false;

            if (IsIncorrectId(order.AuthorId))
            {
                order.AuthorId = order.Author?.Id ?? 0;
            }

            bool isAuthorIdExist = Users.Any(u => u.Profile.Id == order.AuthorId);

            bool isProductEmpty = string.IsNullOrWhiteSpace(order.Product);

            if (!isAuthorIdExist || isProductEmpty)
            {
                return false;
            }
            else
            {
                if (string.IsNullOrEmpty(order.Status))
                {
                    order.Status = OrderStatus.INITIALIZE;
                }

                order.Author = null;

                Orders.Add(order);
                DataBase.SaveChanges();
                return true;
            }
        }

        public Order GetOrder(int id, bool useTracking)
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

        public IList<Order> GetOrders(int userId, bool useTracking)
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

        public IList<Order> GetAllOrders(bool useTracking)
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
                    var originalOrder = GetOrder(order.Id, true);
                    originalOrder.UpdateFrom(order);
                }

                DataBase.SaveChanges();

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
            DataBase.SaveChanges();

            return true;
        }

        #endregion IOrderDirectoryServise implementation

        private bool IsDetached<TEntity>(TEntity entity) where TEntity : class
        {
            var entityState = DataBase.Entry(entity).State;
            return entityState == EntityState.Detached;
        }

        private static bool IsCorrectId(int id)
        {
            return id > 0;
        }

        private static bool IsIncorrectId(int id)
        {
            return !IsCorrectId(id);
        }
    }
}
