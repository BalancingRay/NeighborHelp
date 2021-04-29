using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeighborHelp.Models
{
    public class Claim
    {
        public int ID { get; set; }
        public int AuthorID { set; get; }
        public User Author { set; get; }
        public User Client { set; get; }

        public string Product { set; get; }
        public string ProductDescription { set; get; }
        public double Cost { set; get; }

        public string ClaimType { set; get; }

        public string Status { get; set; }
    }
}
