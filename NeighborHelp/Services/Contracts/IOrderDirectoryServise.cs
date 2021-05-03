using NeighborHelpModels.Models;
using System.Collections.Generic;

namespace NeighborHelp.Services.Contracts
{
    public interface IOrderDirectoryServise
    {
        public bool TryAddOrder(Order order);
        public Order GetOrder(int id, bool useTracking = ContractConsts.DefaultTracking);
        public IList<Order> GetOrders(int userId, bool useTracking = ContractConsts.DefaultTracking);

        public IList<Order> GetAllOrders(bool useTracking = ContractConsts.DefaultTracking);

        public bool TryPutOrder(Order order);

        public bool TryRemoveOrder(int id);
    }
}
