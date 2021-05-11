using NeighborHelpModels.Models;
using NeighborHelpModels.Models.Consts;
using System.Collections.Generic;
using System.Linq;

namespace NeighborHelpModels.Extentions
{
    public static class OrderExtention
    {
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

        public static void UpdateFrom(this Order target, Order source)
        {
            target.Id = source.Id;
            target.Product = source.Product;
            target.ProductDescription = source.ProductDescription;
            target.Cost = source.Cost;
            target.AuthorId = source.AuthorId;
            target.OrderType = source.OrderType;
            target.Status = source.Status;
            target.ClientId = source.ClientId;

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

        public static bool IsEquals(this Order a, Order b)
        {
            if (a != null && b != null)
            {
                bool isEqual = a.Id == b.Id
                  && a.Product == b.Product
                  && a.ProductDescription == b.ProductDescription
                  && a.Status == b.Status
                  && a.OrderType == b.OrderType
                  && a.AuthorId == b.AuthorId
                  && a.ClientId == b.ClientId;

                if (!isEqual)
                    return false;

                isEqual = IsEquals(a.Author, b.Author);
                return isEqual;
            }
            else if (a == null && b == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool IsEquals(this UserProfile a, UserProfile b)
        {
            if (a != null && b != null)
            {
                bool isEqual = a.Name == b.Name
                    && a.Address == b.Address
                    && a.PhoneNumber == b.PhoneNumber;

                return isEqual;
            }
            else if (a == null && b == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static IEnumerable<string> GetAllTypes(this Order obj)
        {
            return new string[] 
            { 
                OrderTypes.SELL, 
                OrderTypes.BUY 
            };
        }

        public static IEnumerable<string> GetAllStatuses(this Order obj)
        {
            return new string[] 
            { 
                OrderStatus.INITIALIZE, 
                OrderStatus.ACTIVE,
                OrderStatus.RESPONSED,
                OrderStatus.CLOSED, 
                OrderStatus.FINISHED 
            };
        }
    }
}
