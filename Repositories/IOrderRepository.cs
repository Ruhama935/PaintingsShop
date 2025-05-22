using Entities;

namespace Repositories
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrder(Order order);
        Task<IEnumerable<Order>> GetOrders(int id);
    }
}