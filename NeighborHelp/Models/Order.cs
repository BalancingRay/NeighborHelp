using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NeighborHelp.Models
{
    public class Order
    {
        public int ID { get; set; }
        [NotMapped]
        public int AuthorId { set; get; }
        [NotMapped]
        public UserProfile Author { set; get; }
        [NotMapped]
        public UserProfile Client { set; get; }

        public string Product { set; get; }
        public string ProductDescription { set; get; }
        public double Cost { set; get; }

        public string OrderType { set; get; }

        public string Status { get; set; }
    }
}
