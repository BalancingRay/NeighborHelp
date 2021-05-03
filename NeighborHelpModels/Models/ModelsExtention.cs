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

            var newUser = new User();
            newUser.UpdateFrom(user);           

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
            if (order == null)
                return null;

            var newOrder = new Order();
            newOrder.UpdateFrom(order);

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

        public static void UpdateFrom(this User target, User source)
        {
            target.Id = source.Id;
            target.UserName = source.UserName;
            target.Login = source.Login;
            target.Password = source.Password;
            target.Role = source.Role;

            if (source.Profile != null)
            {
                if (target.Profile == null)
                    target.Profile = new UserProfile();

                target.Profile.Name = source.Profile.Name;
                target.Profile.Address = source.Profile.Address;
                target.Profile.PhoneNumber = source.Profile.PhoneNumber;
                target.Profile.Id = source.Profile.Id;
            }
        }

        public static void UpdateFrom(this Order target, Order source)
        {
            target.Id = source.Id;
            target.Product = source.Product;
            target.ProductDescription = source.ProductDescription;
            target.Cost = source.Cost;
            target.AuthorId = source.AuthorId;
            target.OrderType = source.OrderType;
            target.Status = source.Status;

            if (source.Author != null)
            {
                if (target.Author == null)
                    target.Author = new UserProfile();

                target.Author.Id = source.Author.Id;
                target.Author.Name = source.Author.Name;
                target.Author.Address = source.Author.Address;
                target.Author.PhoneNumber = source.Author.PhoneNumber;
            }
        }
    }
}
