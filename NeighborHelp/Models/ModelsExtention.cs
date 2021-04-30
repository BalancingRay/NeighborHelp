using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeighborHelp.Models
{
    public static class ModelsExtention
    {
        public static User Dublicate(this User user)
        {
            if (user == null)
                return null;

            var newUser = new User()
            {
                Id = user.Id,
                UserName = user.UserName,
                Login = user.Login,
                Password = user.Password,
                Role = user.Role,
            };

            if (user.Profile != null)
            {
                newUser.Profile = new UserProfile()
                {
                    Name = user.Profile.Name,
                    Address = user.Profile.Address,
                    PhoneNumber = user.Profile.PhoneNumber,
                    Id = user.Profile.Id
                };
            }

            return newUser;
        }

        public static IList<User> Dublicate(this IEnumerable<User> users)
        {
            var newUsers = new List<User>();

            if (users == null || users.Count() == 0)
                return newUsers;

            foreach (var item in users)
            {
                newUsers.Add(Dublicate(item));
            }

            return newUsers;
        }

        public static Order Dublicate(this Order order)
        {
            var newOrder = new Order()
            {
                ID = order.ID,
                Product = order.Product,
                ProductDescription = order.ProductDescription,
                Cost = order.Cost,
                AuthorId = order.AuthorId,
                OrderType = order.OrderType,
                Status = order.Status
            };

            if (order.Author != null)
            {
                newOrder.Author = new UserProfile()
                {
                    Name = order.Author.Name,
                    Address = order.Author.Address,
                    Id = order.Author.Id,
                    PhoneNumber = order.Author.PhoneNumber
                };
            }

            return newOrder;
        }

        public static IList<Order> Dublicate(this IEnumerable<Order> orders)
        {
            var newOrders = new List<Order>();

            if (orders == null || orders.Count() == 0)
                return newOrders;

            foreach (var item in orders)
            {
                newOrders.Add(Dublicate(item));
            }

            return newOrders;
        }
    }
}
