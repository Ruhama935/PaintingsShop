using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly PaintingsShopContext _paintingsShopContext;
        public CategoryRepository(PaintingsShopContext paintingsShopContext)
        {
            _paintingsShopContext = paintingsShopContext;
        }

        public async Task<List<Category>> GetCategories()
        {
            return await _paintingsShopContext.Categories.ToListAsync();
        }
    }
}
