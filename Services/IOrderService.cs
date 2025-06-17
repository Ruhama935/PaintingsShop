using Entities;
using DTOs;

namespace Services
{
    public interface IOrderService
    {
        Task<OrderDTO> CreateOrder(OrderDTO order);
        Task<List<OrderDTO>> GetOrders(int id);
    }
}