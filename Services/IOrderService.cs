using Entities;

namespace Services
{
    public interface IOrderService
    {
        Task<Order> CreateOrder(Order order);
        Task<IEnumerable<Order>> GetOrders(int id);
    }
}