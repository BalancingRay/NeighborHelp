using NeighborHelp.Models;
using NeighborHelp.Models.Consts;
using NeighborHelp.Services.Contracts;

namespace NeighborHelp.Services
{
    public class TestDataFiller
    {

        private IOrderDirectoryServise _orders;
        private IUserDirectoryServise _users;

        public TestDataFiller(IOrderDirectoryServise orders, IUserDirectoryServise users)
        {
            _orders = orders;
            _users = users;
        }

        public void FillAll()
        {
            FillUsers();
            FillOrders(_users.GetUser(1));
            FillOrders2(_users.GetUser(2));
        }

        public void FillIfEmpty()
        {
            if (_users.GetUsers().Count == 0)
            {
                FillUsers();
            }

            if (_orders.GetAllOrders().Count == 0)
            {
                FillOrders(_users.GetUser(1, true));
                FillOrders2(_users.GetUser(2, true));
            }
        }

        private void FillUsers()
        {
            var adminProfile = new UserProfile() { Name = "Vasil", Address = "Minsk" };
            var user1Profile = new UserProfile() { Name = "Ilia", Address = "Krasnaya str. 28", PhoneNumber = "299-87-65" };

            var admin = new User()
            {
                Login = "admin",
                Password = "admin",
                Role = UserRoles.ADMIN,
                UserName = "SuperAdmin",
                Profile = adminProfile
            };
            var user = new User()
            {
                Login = "user1",
                Password = "1234",
                Role = UserRoles.USER,
                UserName = "TestUser",
                Profile = user1Profile
            };

            _users.TryAddUser(admin);
            _users.TryAddUser(user);
        }

        private void FillOrders(User user)
        {
            if (user?.Profile == null)
                return;

            var userOrder = new Order()
            {
                Author = user.Profile,
                AuthorId = user.Id,
                OrderType = OrderTypes.SELL,
                Product = "Диван, б/у",
                ProductDescription = "Коричневый, мягкая обивка, состояние 7 из 10",
                Cost = 50
            };

            var userOrder2 = new Order()
            {
                Author = user.Profile,
                AuthorId = user.Id,
                OrderType = OrderTypes.BUY,
                Product = "Грамофон",
                ProductDescription = "Рабочий. Для проигрывания виниловых пластинок, можно полностью механический",
                Cost = 80
            };

            _orders.TryAddOrder(userOrder);
            _orders.TryAddOrder(userOrder2);
        }

        private void FillOrders2(User user)
        {
            if (user?.Profile == null)
                return;

            var userOrder = new Order()
            {
                Author = user.Profile,
                AuthorId = user.Id,
                OrderType = OrderTypes.SELL,
                Product = "Капуста",
                ProductDescription = "Среднего размера. Хорошее качество. Для засолки",
                Cost = 3
            };

            var userOrder2 = new Order()
            {
                Author = user.Profile,
                AuthorId = user.Id,
                OrderType = OrderTypes.BUY,
                Product = "3 литровые банки",
                ProductDescription = "Стреклянные банки 3 литра. б/у. Без сколов и трещин. Под закатки",
                Cost = 3
            };

            _orders.TryAddOrder(userOrder);
            _orders.TryAddOrder(userOrder2);
        }
    }
}
