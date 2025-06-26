using Microsoft.AspNetCore.Mvc;
using Services;
using System.Threading.Tasks;
using DTOs;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860//

namespace PaintingsShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        ICategoryService _categoryService;
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        // GET: api/<CategoriesController>//
        [HttpGet]
        public async Task<List<CategoryDTO>> Get()//clean code - use meaningful names
        {
            return await _categoryService.GetCategories();
        }
    }
}
