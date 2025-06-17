using Entities;

namespace Repositories
{
    public interface IProductRepository
    {
        Task<Product> GerProductById(int id);
        Task<List<Product>> GetAllProducts();
        Task<List<Product>> GetProductsFiltered(int? categoryId, decimal? minPrice, decimal? maxPrice);

    }
}