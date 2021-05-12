using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NeighborHelpModels.Models;
using NeighborHelpModels.Models.Consts;
using NeighborHelp.Services.Contracts;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using NeighborHelp.Utils;
using NeighborHelpAPI.Consts;
using NeighborHelp.Properties;

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
         ActionName(UserControllerConsts.GET_CURRENT_ACTION)]
        [Authorize(AuthenticationSchemes = AuthenticationPropForAttributes.Value)]
        public ActionResult<User> Current()
        {
            User user = null;
            if (AuthorizationHelper.TryGetCurrentUserId(HttpContext?.User, out int id))
            {
                user = _userDirectory.GetUser(id);
            }

            if (user != null)
            {
                return new ActionResult<User>(user);
            }
            else
            {
                return new NotFoundResult();
            }
        }

        [HttpGet("{id}"),
         Authorize(Roles = UserRoles.ADMIN),
         ActionName(UserControllerConsts.GET_ACTION)]
        [Authorize(AuthenticationSchemes = AuthenticationPropForAttributes.Value)]
        public ActionResult<User> Get(int id)
        {
            var user = _userDirectory.GetUser(id);

            if (user != null)
            {
                return new ActionResult<User>(user);
            }
            else
            {
                return new NotFoundResult();
            }
        }

        [HttpGet,
         Authorize(Roles = UserRoles.ADMIN),
         ActionName(UserControllerConsts.GET_ALL_ACTION)]
        [Authorize(AuthenticationSchemes = AuthenticationPropForAttributes.Value)]
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

        [Authorize(AuthenticationSchemes = AuthenticationPropForAttributes.Value)]
        [ActionName(UserControllerConsts.UPDATE_ACTION)]
        [HttpPut]
        public ActionResult<User> Put(User user)
        {
            if (user == null)
            {
                return new NoContentResult();
            }

            bool isNotCurrentUser = AuthorizationHelper.TryGetCurrentUserId(HttpContext?.User, out int id) 
                && id != user.Id;
            bool isNotAdmin = AuthorizationHelper.GetCurrentUserRole(HttpContext?.User) != UserRoles.ADMIN;

            if (isNotCurrentUser && isNotAdmin)
            {
                return new StatusCodeResult(StatusCodes.Status403Forbidden);
            }

            bool succeed = _userDirectory.TryPutUser(user);

            if (succeed)
            {
                return new ActionResult<User>(user);
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
                return new ActionResult<User>(user);
            }
            else
            {
                return new BadRequestResult();
            }
        }
    }
}
