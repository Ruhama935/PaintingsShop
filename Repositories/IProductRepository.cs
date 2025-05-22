using Entities;

namespace Repositories
{
    public interface IProductRepository
    {
        Task<Product> GerProductById(int id);
        Task<List<Product>> GetAllProducts();
    }
}