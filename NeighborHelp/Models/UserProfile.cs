using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeighborHelp.Models
{
    public class UserProfile
    {
        public int Id { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public string Name { get; set; }

        public List<Order> Orders { set; get; }

        public UserProfile()
        {
            Orders = new List<Order>();
        }
    }
}
