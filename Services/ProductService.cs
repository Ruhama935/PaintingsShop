using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories;
using Entities;

namespace Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<List<Product>> GetAllProducts()
        {
            return await _productRepository.GetAllProducts();
        }
        public async Task<Product> GerProductById(int id)
        {
            return await _productRepository.GerProductById(id);
        }
    }
}
