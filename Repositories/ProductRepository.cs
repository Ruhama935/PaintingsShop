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
            return await _paintingsShopContext.Products.ToListAsync();
        }

        public async Task<Product> GerProductById(int id)
        {
            return await _paintingsShopContext.Products.FindAsync(id);
        }
    }
}
