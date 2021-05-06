namespace NeighborHelpModels.Models
{
    public class User
    {
        public int Id { set; get; }

        public string Login { set; get; }
        public string UserName { set; get; }

        public string Password { set; get; }

        public string Role { set; get; }

        public UserProfile Profile { set; get; }
    }
}
