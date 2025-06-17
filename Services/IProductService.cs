using Entities;
using DTOs;

namespace Services
{
    public interface IProductService
    {
        Task<ProductDTO> GerProductById(int id);
        Task<List<ProductDTO>> GetAllProducts();
        Task<List<ProductDTO>> GetProductsFiltered(int? categoryId, decimal? minPrice, decimal? maxPrice);

    }
}