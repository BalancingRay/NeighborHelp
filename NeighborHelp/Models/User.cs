using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeighborHelp.Models
{
    public class User
    {
        public int ID { set; get; }
        public string Login { set; get; }
        public string UserName { set; get; }
        public string Password { set; get; }
        public string Role { set; get; }

        public UserProfile Profile { set; get; }

        public List<Claim> Claims { set; get; }
    }
}
