using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NeighborHelpModels.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { set; get; }

        [Required]
        public string Login { set; get; }
        public string UserName { set; get; }

        public string Password { set; get; }

        public string Role { set; get; }

        public UserProfile Profile { set; get; }
    }
}
