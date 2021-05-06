﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NeighborHelp.Controllers.Consts;
using NeighborHelpModels.Models;
using NeighborHelp.Services.Contracts;
using System.Collections.Generic;
using NeighborHelp.Utils;
using NeighborHelpModels.Models.Consts;
using Microsoft.AspNetCore.Http;

namespace NeighborHelp.Controllers
{
    [ApiController]
    [Route("api/[Controller]/[Action]")]
    public class OrderController : Controller
    {
        private IOrderDirectoryServise _orderDirectory;

        public OrderController(IOrderDirectoryServise service)
        {
            _orderDirectory = service;
        }

        [HttpGet]
        [ActionName(OrderControllerConsts.GET_ALL_ACTION)]
        public ActionResult<IEnumerable<Order>> GetAll()
        {
            var orders = _orderDirectory.GetAllOrders();
            return new ActionResult<IEnumerable<Order>>(orders);
        }

        [HttpGet("{userId}")]
        [Authorize]
        [ActionName(OrderControllerConsts.GET_BY_USER_ACTION)]
        public ActionResult<IEnumerable<Order>> GetByUser(int userId)
        {
            var orders = _orderDirectory.GetOrders(userId);
            return new ActionResult<IEnumerable<Order>>(orders);
        }

        [HttpGet("{id}")]
        [ActionName(OrderControllerConsts.GET_ACTION)]
        public ActionResult<Order> Get(int id)
        {
            var order = _orderDirectory.GetOrder(id);

            if (order != null)
            {
                return new ActionResult<Order>(order);
            }
            else
            {
                return new NotFoundResult();
            }
        }

        [Authorize]
        [HttpPut]
        [ActionName(OrderControllerConsts.PUT_ACTION)]
        public ActionResult<Order> Put(Order order)
        {
            if (order == null)
            {
                return new NoContentResult();
            }

            int originalAuthorId = _orderDirectory.GetOrder(order.Id)?.AuthorId ?? 0;

            bool isNotCurrentUser = AuthorizationHelper.TryGetCurrentUserId(HttpContext?.User, out int id)
                && id != originalAuthorId;
            bool isNotAdmin = AuthorizationHelper.GetCurrentUserRole(HttpContext?.User) != UserRoles.ADMIN;

            if (isNotCurrentUser && isNotAdmin)
            {
                return new StatusCodeResult(StatusCodes.Status403Forbidden);
            }

            bool succeed = _orderDirectory.TryPutOrder(order);

            if (succeed)
            {
                return new ActionResult<Order>(order);
            }
            else
            {
                return new BadRequestResult();
            }
        }

        [Authorize]
        [HttpPost]
        [ActionName(OrderControllerConsts.ADD_ACTION)]
        public ActionResult<Order> Post(Order order)
        {
            if (order == null)
            {
                return new NoContentResult();
            }

            bool succeed = false;

            if (AuthorizationHelper.TryGetCurrentUserId(HttpContext?.User, out int id))
            {
                order.AuthorId = id;
                succeed = _orderDirectory.TryAddOrder(order);
            }

            if (succeed)
            {
                return new ActionResult<Order>(order);
            }
            else
            {
                return new BadRequestResult();
            }
        }
    }
}
