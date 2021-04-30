using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeighborHelp.Models
{
    public static class ModelsExtention
    {
        public static User Dublicate(this User user)
        {
            if (user == null)
                return null;

            var newUser = new User()
            {
                Id = user.Id,
                UserName = user.UserName,
                Login = user.Login,
                Password = user.Password,
                Role = user.Role,
            };

            if (user.Profile != null)
            {
                newUser.Profile = new UserProfile()
                {
                    Name = user.Profile.Name,
                    Address = user.Profile.Address,
                    PhoneNumber = user.Profile.PhoneNumber,
                    Id = user.Profile.Id
                };
            }

            return newUser;
        }

        public static IList<User> Dublicate(this IEnumerable<User> users)
        {
            var newUsers = new List<User>();

            if (users == null || users.Count() == 0)
                return newUsers;

            foreach (var item in users)
            {
                newUsers.Add(Dublicate(item));
            }

            return newUsers;
        }
    }
}
