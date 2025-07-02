using Microsoft.AspNetCore.Mvc;
using Repositories;
using Services;
using DTOs;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PaintingsShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        IProductService _productService;
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }
        // GET: api/<ProductController>
        [HttpGet]
        public async Task<List<ProductDTO>> Get()
        {
            return await _productService.GetAllProducts();
        }

        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        public async Task<ProductDTO> Get(int id)
        {
            return await _productService.GerProductById(id);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? categoryId, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice)
        {
            var products = await _productService.GetProductsFiltered(categoryId, minPrice, maxPrice);
            return Ok(products);
        }
    }
}
