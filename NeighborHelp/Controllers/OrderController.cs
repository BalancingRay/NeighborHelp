using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NeighborHelpModels.Models;
using NeighborHelp.Services.Contracts;
using System.Collections.Generic;
using NeighborHelp.Utils;
using NeighborHelpModels.Models.Consts;
using Microsoft.AspNetCore.Http;
using NeighborHelpAPI.Consts;

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
        //[Authorize]
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


        [Authorize]
        [HttpPut]
        [ActionName(OrderControllerConsts.RESPONSE_ACTION)]
        public ActionResult<Order> Responce(Order order)
        {
            if (order == null)
            {
                return new NoContentResult();
            }

            bool hasAutorId = AuthorizationHelper.TryGetCurrentUserId(HttpContext?.User, out int authorId);

            if (!hasAutorId)
            {
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            }

            var originalOrder = _orderDirectory.GetOrder(order.Id);

            bool isOrderChanged = !IsEquals(originalOrder, order);

            bool isNotValidStatus = string.Compare(originalOrder?.Status, OrderStatus.ACTIVE) != 0;

            if (isOrderChanged || isNotValidStatus)
            {
                return new StatusCodeResult(StatusCodes.Status403Forbidden);
            }

            order.ClientId = authorId;
            order.Status = OrderStatus.RESPONSED;

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

        //TODO update extension or add IEqutable interface
        private bool IsEquals(Order a, Order b)
        {
            if (a != null && b != null)
            {
                bool isEqual = a.Id == b.Id
                  && a.Product == b.Product
                  && a.ProductDescription == b.ProductDescription
                  && a.Status == b.Status
                  && a.OrderType == b.OrderType;

                if (!isEqual)
                    return false;

                if (a.Author != null && b.Author != null)
                {
                    isEqual = a.Author.Name == b.Author.Name
                        && a.Author.Address == b.Author.Address
                        && a.Author.PhoneNumber == b.Author.PhoneNumber;

                    return isEqual;
                }
                else if (a.Author == null && b.Author == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (a == null && b == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
