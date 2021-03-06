namespace NeighborHelpModels.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int AuthorId { set; get; }

        public UserProfile Author { set; get; }
        public int ClientId { set; get; }

        public string Product { set; get; }
        public string ProductDescription { set; get; }
        public double Cost { set; get; }

        public string OrderType { set; get; }

        public string Status { get; set; }
    }
}
