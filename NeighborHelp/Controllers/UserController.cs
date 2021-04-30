using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NeighborHelp.Controllers.Consts;
using NeighborHelp.Models;
using NeighborHelp.Models.Consts;
using NeighborHelp.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NeighborHelp.Controllers
{
    [ApiController]
    [Route("api/[Controller]/[Action]")]
    public class UserController : Controller
    {
        private IUserDirectoryServise _userDirectory;

        public UserController(IUserDirectoryServise service) 
        {
            _userDirectory = service;
        }

        [HttpGet,
         Authorize,
         ActionName(UserControllerConsts.GET_CURRENT_ACTION)]
        public ActionResult<User> Current()
        {
            var claimId = HttpContext.User.Claims.FirstOrDefault(cl => cl.Type == ClaimsIdentity.DefaultNameClaimType)?.Value;
            User user = null;

            if (int.TryParse(claimId, out int id))
            {
                user = _userDirectory.GetUser(id);
            }

            if (user != null)
            {
                return new ObjectResult(user);
            }
            else
            {
                return new NotFoundResult();
            }
        }

        [HttpGet("{id}"),
         Authorize(Roles =UserRoles.ADMIN),
         ActionName((UserControllerConsts.GET_ACTION))]
        public ActionResult<User> Get(int id)
        {
            var user = _userDirectory.GetUser(id);

            if (user != null)
            {
                return new ObjectResult(user);
            }
            else
            {
                return new NotFoundResult();
            }
        }

        [HttpGet,
         Authorize(Roles = UserRoles.ADMIN),
         ActionName(UserControllerConsts.GET_ALL_ACTION)]

        public ActionResult<IEnumerable<User>> GetAll()
        {
            var users = _userDirectory.GetUsers();

            if (users != null)
            {
                return new ActionResult<IEnumerable<User>>(users);
            }
            else
            {
                return new NotFoundResult();
            }
        }

        [Authorize]
        [ActionName(UserControllerConsts.UPDATE_ACTION)]
        [HttpPut]
        public ActionResult<User> Put(User user)
        {
            if (user == null)
            {
                return new NoContentResult();
            }

            bool succeed = _userDirectory.TryPutUser(user);

            if (succeed)
            {
                return new OkObjectResult(user);
            }
            else
            {
                return new BadRequestResult();
            }
        }

        [ActionName(UserControllerConsts.ADD_USER)]
        [HttpPost]
        public ActionResult<User> Post(User user)
        {
            if (user == null)
            {
                return new NoContentResult();
            }

            bool succeed = _userDirectory.TryAddUser(user);

            if (succeed)
            {
                return new OkObjectResult(user);
            }
            else
            {
                return new BadRequestResult();
            }
        }
    }
}
