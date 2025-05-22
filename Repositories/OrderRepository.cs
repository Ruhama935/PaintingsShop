using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly PaintingsShopContext _paintingsShopContext;
        public OrderRepository(PaintingsShopContext paintingsShopContext)
        {
            _paintingsShopContext = paintingsShopContext;
        }

        public async Task<Order> CreateOrder(Order order)
        {
            await _paintingsShopContext.Orders.AddAsync(order);
            await _paintingsShopContext.SaveChangesAsync();
            return order;
        }
        public async Task<IEnumerable<Order>> GetOrders(int id)
        {
            return await _paintingsShopContext.Orders.Where(order => order.Id == id).ToListAsync();
        }
    }
}
