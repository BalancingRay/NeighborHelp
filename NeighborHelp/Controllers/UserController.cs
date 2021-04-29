using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NeighborHelp.Controllers.Consts;
using NeighborHelp.Models;
using NeighborHelp.Models.Consts;
using NeighborHelp.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeighborHelp.Controllers
{
    public class UserController : Controller
    {
        private IUserDirectoryServise _userDirectory;

        public UserController(IUserDirectoryServise service) 
        {
            _userDirectory = service;
        }

        public IActionResult Login(User user)
        {
            bool authorizationResult = true;

            if (authorizationResult)
            {
                return new OkResult();
            }
            else
            {
                return new NotFoundResult();
            }
        }

        [Authorize]
        [ActionName(UserControllerConsts.GET_CURRENT_USER)]
        [HttpGet]
        public IActionResult GetUser()
        {
            int id = -1;//TODO get current user id
            var user = _userDirectory.GetUser(id);

            if (user != null)
            {
                return new JsonResult(user);
            }
            else
            {
                return new NotFoundResult();
            }
        }

        [Authorize(Roles =UserRoles.ADMIN)]
        [ActionName((UserControllerConsts.GET_USER))]
        [HttpGet]
        public IActionResult GetUser(int id)
        {
            var user = _userDirectory.GetUser(id);

            if (user != null)
            {
                return new JsonResult(user);
            }
            else
            {
                return new NotFoundResult();
            }
        }

        [Authorize(Roles = UserRoles.ADMIN)]
        [ActionName(UserControllerConsts.GET_USERS)]
        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _userDirectory.GetUsers();

            if (users != null)
            {
                return new JsonResult(users);
            }
            else
            {
                return new NotFoundResult();
            }
        }

        [Authorize]
        [ActionName(UserControllerConsts.PUT_USER)]
        [HttpPut]
        public IActionResult PutUser(User user)
        {
            if (user == null)
            {
                return new NoContentResult();
            }

            bool succeed = _userDirectory.TryPutUser(user);

            if (succeed)
            {
                return new OkResult();
            }
            else
            {
                return new NoContentResult();
            }
        }

        [ActionName(UserControllerConsts.ADD_USER)]
        [HttpPost]
        public IActionResult AddUser(User user)
        {
            if (user == null)
            {
                return new NoContentResult();
            }

            bool succeed = _userDirectory.TryAddUser(user);

            if (succeed)
            {
                return new OkResult();
            }
            else
            {
                return new NoContentResult();
            }
        }

    }
}
