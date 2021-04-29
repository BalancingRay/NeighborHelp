using NeighborHelp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeighborHelp.Services.Contracts
{
    public interface IOrderDirectoryServise
    {
        public bool TryAddOrder(Order order);
        public Order GetOrder(int id);
        public IList<Order> GetOrders(int userId);

        public IList<Order> GetAllOrders();

        public bool TryPutOrder(Order order);
    }
}
