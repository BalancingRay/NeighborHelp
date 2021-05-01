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
        public Order GetOrder(int id, bool useTracking = false);
        public IList<Order> GetOrders(int userId, bool useTracking = false);

        public IList<Order> GetAllOrders(bool useTracking = false);

        public bool TryPutOrder(Order order);
    }
}
