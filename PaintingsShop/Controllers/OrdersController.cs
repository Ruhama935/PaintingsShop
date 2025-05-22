using Microsoft.AspNetCore.Mvc;
using Entities;
using Repositories;
using Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PaintingsShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        IOrderService _orderService;
        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        // GET api/<OrdersController>/5
        [HttpGet("{id}")]
        public async Task<IEnumerable<Order>> Get(int id)
        {
            return await _orderService.GetOrders(id);
        }

        // POST api/<OrdersController>
        [HttpPost]
        public async Task<Order> Post([FromBody] Order order)
        {
            return await _orderService.CreateOrder(order);
        }
    }
}
