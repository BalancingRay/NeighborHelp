using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NeighborHelpModels.Models;
using NeighborHelpInfrastructure.ServiceContracts;
using System.Collections.Generic;
using NeighborHelp.Utils;
using NeighborHelpModels.Models.Consts;
using Microsoft.AspNetCore.Http;
using NeighborHelpAPI.Consts;
using NeighborHelpModels.Extentions;
using NeighborHelpInfrastucture.Utils;

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

        [Authorize(AuthenticationSchemes = AuthorizeAttributeHelper.Value)]
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

        [Authorize(AuthenticationSchemes = AuthorizeAttributeHelper.Value)]
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


        [Authorize(AuthenticationSchemes = AuthorizeAttributeHelper.Value)]
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
            bool isOrderChanged = !order.IsEquals(originalOrder);
            bool isNotValidStatus = !string.Equals(originalOrder?.Status, OrderStatus.ACTIVE, System.StringComparison.OrdinalIgnoreCase);

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
    }
}
