using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories;
using Entities;
using DTOs;
using AutoMapper;

namespace Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        public OrderService(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }
        public async Task<OrderDTO> CreateOrder(OrderDTO order)
        {
            var newOrder = await _orderRepository.CreateOrder(_mapper.Map < Order > (order));
            return _mapper.Map<OrderDTO>(newOrder);
        }
        public async Task<List<OrderDTO>> GetOrders(int id)
        {
            var orders = await _orderRepository.GetOrders(id);
            return _mapper.Map<List<OrderDTO>>(orders);
        }
    }
}
