using Microsoft.AspNetCore.Mvc;
using NeighborHelp.Controllers.Consts;
using NeighborHelp.Models;
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

        //TODO authorized role
        [HttpGet(UserControllerConsts.GET_CURRENT_USER)]
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

        //TODO admin role
        [HttpGet(UserControllerConsts.GET_USER)]
        public IActionResult GetUser(string id)
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

        //TODO admin role
        [HttpGet(UserControllerConsts.GET_USERS)]
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

        //TODO authorized role
        [HttpPut(UserControllerConsts.PUT_USER)]
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

        [HttpPost(UserControllerConsts.ADD_USER)]
        public IActionResult PostUser(User user)
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
