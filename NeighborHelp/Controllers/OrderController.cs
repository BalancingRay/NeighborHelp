using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NeighborHelp.Models;
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
    public class OrderController : Controller
    {
        private IOrderDirectoryServise _orderDirectory;

        public OrderController(IOrderDirectoryServise service)
        {
            _orderDirectory = service;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Order>> GetAll()
        {
            var orders = _orderDirectory.GetAllOrders();
            return new ActionResult<IEnumerable<Order>>(orders);
        }

        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<IEnumerable<Order>> GetByUser(int id)
        {
            var orders = _orderDirectory.GetOrders(id);
            return new ActionResult<IEnumerable<Order>>(orders);
        }

        [HttpGet("{id}")]
        public ActionResult<Order> Get(int id)
        {
            var order = _orderDirectory.GetOrder(id);

            if (order != null)
            {
                return new ObjectResult(order);
            }
            else
            {
                return new NotFoundResult();
            }
        }

        [Authorize]
        [HttpPut]
        public ActionResult<Order> Put(Order order)
        {
            if (order == null)
            {
                return new NoContentResult();
            }

            bool succeed = _orderDirectory.TryPutOrder(order);

            if (succeed)
            {
                return new OkObjectResult(order);
            }
            else
            {
                return new BadRequestResult();
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult<Order> Post(Order order)
        {
            if (order == null)
            {
                return new NoContentResult();
            }

            var claimId = HttpContext.User.Claims.FirstOrDefault(cl => cl.Type == ClaimsIdentity.DefaultNameClaimType)?.Value;
            bool succeed = false;

            if (int.TryParse(claimId, out int id))
            {
                order.AuthorID = id;
                succeed = _orderDirectory.TryAddOrder(order);
            }

            if (succeed)
            {
                return new OkObjectResult(order);
            }
            else
            {
                return new BadRequestResult();
            }
        }
    }
}
