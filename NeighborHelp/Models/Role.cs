using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeighborHelp.Models
{
    public class Role
    {
        public int ID { set; get; }
        public string Name { set; get; }

        public List<User> Users { set; get; }

        public Role()
        {
            Users = new List<User>();
        }
    }
}
