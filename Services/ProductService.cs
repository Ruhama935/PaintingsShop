using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories;
using Entities;
using Org.BouncyCastle.Crypto;
using AutoMapper;
using DTOs;
//using Mapping;

namespace Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }
        public async Task<List<ProductDTO>> GetAllProducts()
        {
            var products = await _productRepository.GetAllProducts();
            var productDTOs = _mapper.Map<List<ProductDTO>>(products);
            return productDTOs;
        }
        public async Task<ProductDTO> GerProductById(int id)
        {
            var product = await _productRepository.GerProductById(id);
            var productDTOs = _mapper.Map<ProductDTO>(product);
            return productDTOs;
        }
        public async Task<List<ProductDTO>> GetProductsFiltered(int? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            var products = await _productRepository.GetProductsFiltered(categoryId, minPrice, maxPrice);
            var productDTOs = _mapper.Map<List<ProductDTO>>(products);
            return productDTOs;
        }
    }
}
