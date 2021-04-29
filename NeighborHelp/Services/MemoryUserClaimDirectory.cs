using NeighborHelp.Models;
using NeighborHelp.Models.Consts;
using NeighborHelp.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeighborHelp.Services
{
    public class MemoryUserClaimDirectory: IUserDirectoryServise, IClaimDirectoryServise
    {
        private List<User> Users;
        private List<Role> Roles;
        private List<Claim> Claims;

        public MemoryUserClaimDirectory()
        {
            Users = new List<User>();
            Claims = new List<Claim>();
            Roles = new List<Role>();

            FillUsers();
            FillClaims(Users.FirstOrDefault());
        }

        private void FillUsers()
        {
            var userRole = new Role() { Name = UserRoles.USER, ID=0 };
            var adminRole = new Role() { Name = UserRoles.ADMIN, ID=1 };

            Roles.Add(userRole);
            Roles.Add(adminRole);

            var admin = new User() { Login = "admin", Password = "admin", Role = adminRole, RoleId = adminRole.ID, UserName = "Admin" };
            var user = new User() { Login = "user1", Password = "1234", Role = userRole, RoleId = userRole.ID, UserName = "TestUser" };

            TryAddUser(admin);
            TryAddUser(user);
        }

        private void FillClaims(User user)
        {
            var userClaim = new Claim()
            {
                Author = user,
                AuthorID = user.Id,
                ClaimType = ClaimTypes.SELL,
                Product = "Диван, б/у",
                ProductDescription = "Коричневый, мягкая обивка, состояние 7 из 10",
                Cost = 50
            };
        }

        #region IClaimDirectoryServise implementation

        public bool TryAddClaim(Claim claim)
        {
            bool isClaimNotInitialized = 
                string.IsNullOrWhiteSpace(claim?.Author?.Login) 
                || string.IsNullOrWhiteSpace(claim?.Product);

            if (isClaimNotInitialized)
            {
                return false;
            }
            else
            {
                int lastID = Claims.Last()?.ID ?? 0;
                claim.ID = lastID++;
                Claims.Add(claim);

                return true;
            }
        }

        public Claim GetClaim(int id)
        {
            return Claims.FirstOrDefault(cl => cl.ID == id);
        }

        public IList<Claim> GetClaims(int userId)
        {
            return Claims.Where(cl => cl.Author.Id == userId).ToList();
        }

        public IList<Claim> GetAllClaims()
        {
            return Claims.ToList();
        }

        public bool TryPutClaim(Claim claim)
        {
            var oldClaim = Claims.FirstOrDefault(cl => (cl.ID == claim.ID
                                              && cl.AuthorID == claim?.AuthorID));

            if (oldClaim != null)
            {
                Claims[Claims.IndexOf(oldClaim)] = claim;

                return true;
            }

            return false;
        }

        #endregion IClaimDirectoryServise implementation

        #region IUserDirectoryServise implementation

        public bool TryAddUser(User user)
        {
            bool isUserNameEmpty = string.IsNullOrWhiteSpace(user?.Login);
            bool isUserExist = Users.Any(u => u.Login == user?.Login);

            if (isUserNameEmpty || isUserExist)
            {
                return false;
            }
            else
            {
                int lastID = Users.Last()?.Id ?? 0;
                user.Id = lastID++;
                Users.Add(user);
                UpdateRoles(user);

                return true;
            }
        }

        private void UpdateRoles(User user)
        {
            var role = Roles.FirstOrDefault(r => r.Name == user.Role?.Name);

            if (role != null 
                && !role.Users.Any(u => u.Id == user.Id))
            {
                role.Users.Add(user);
            }
        }

        public User GetUser(int id)
        {
            return Users.FirstOrDefault(u => u.Id == id);
        }

        public IList<User> GetUsers()
        {
            return Users.ToList();
        }

        public bool TryPutUser(User user)
        {
            var oldUser = Users.FirstOrDefault( u => (u.Login == user?.Login 
                                                && u.Id == user?.Id));

            if (oldUser !=null)
            {
                Users[Users.IndexOf(oldUser)] = user;

                return true;
            }

            return false;
        }

        #endregion IUserDirectoryServise
    }
}
