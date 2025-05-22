using Entities;

namespace Services
{
    public interface IProductService
    {
        Task<Product> GerProductById(int id);
        Task<List<Product>> GetAllProducts();
    }
}