using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Repositories;

namespace Services
{
    internal class ProductServise : IProductServise
    {
        private readonly IProductRepository _productRepository;
        public ProductServise(IProductRepository productRepository)
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
