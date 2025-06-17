using Microsoft.AspNetCore.Mvc;
using Repositories;
using DTOs;
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
        public async Task<List<OrderDTO>> Get(int id)
        {
            return await _orderService.GetOrders(id);
        }

        // POST api/<OrdersController>
        [HttpPost]
        public async Task<OrderDTO> Post([FromBody] OrderDTO order)
        {
            return await _orderService.CreateOrder(order);
        }
    }
}
