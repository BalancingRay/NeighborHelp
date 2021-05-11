using NeighborHelpModels.Models;
using System.Collections.Generic;
using System.Linq;

namespace NeighborHelpModels.Extentions
{
    public static class UserExtentions
    {
        public static User Dublicate(this User user)
        {
            if (user == null)
                return null;

            var newUser = new User();
            newUser.UpdateFrom(user);

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

        public static void UpdateFrom(this User target, User source)
        {
            target.Id = source.Id;
            target.UserName = source.UserName;
            target.Login = source.Login;
            target.Password = source.Password;
            target.Role = source.Role;

            if (source.Profile != null)
            {
                if (target.Profile == null)
                    target.Profile = new UserProfile();

                target.Profile.Name = source.Profile.Name;
                target.Profile.Address = source.Profile.Address;
                target.Profile.PhoneNumber = source.Profile.PhoneNumber;
                target.Profile.Id = source.Profile.Id;
            }
        }

    }
}
