using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly PaintingsShopContext _paintingsShopContext;
        public ProductRepository(PaintingsShopContext paintingsShopContext)
        {
            _paintingsShopContext = paintingsShopContext;
        }

        public async Task<List<Product>> GetAllProducts()
        {
            return await GetProductsFiltered(null, null, null);
        }

        public async Task<Product> GerProductById(int id)
        {
            return await _paintingsShopContext.Products.FindAsync(id);
        }

        public async Task<List<Product>> GetProductsFiltered(int? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            var query = _paintingsShopContext.Products.AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            return await query.ToListAsync();
        }
    }
}
