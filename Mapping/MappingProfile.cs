using AutoMapper;
using DTOs;
using Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<Order, OrderDTO>().ReverseMap();
            //CreateMap<OrderItem, OrderItemDto>().ReverseMap();
        }
    }
}
