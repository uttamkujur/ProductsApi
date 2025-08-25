using Microsoft.AspNetCore.Mvc;
using Products.Application;
using Products.Application.Dto;
using Products.Domain;
using Products.Infrastructure;


namespace Products.Api
{
    [ApiController]
    [Route("api/products")]

    public class Controller : ControllerBase
    {
        private readonly ProductDbContext _context;

        private readonly IProductService _productService;

        public Controller(ProductDbContext context, IProductService productService)
        {
            _context = context;
            _productService = productService;
        }

        [HttpGet]
        public async Task< ActionResult<IEnumerable<Product>>> GetProducts()
        {            
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet]
        [Route("id")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound(new { message = $"Product with ID {id} not found" });

            return product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(CreateProductDto product)
        {
            if (ModelState.IsValid)
            {
                await _productService.AddProductAsync(product);
                return CreatedAtAction(nameof(GetProduct), product);
            }
            return BadRequest(ModelState);
        }

        [HttpPut("id")]
        public async Task<IActionResult> PutProduct([FromQuery] int id, UpdateProductDto product)
        {
            if (id != product.Id)
                return BadRequest();

            var result = await _productService.UpdateProductAsync(product);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPut("decrement-stock/{id}/{quantity}")]
        public async Task<IActionResult> DecrementStock(int id, int quantity)
        {
            if (quantity <= 0)
                return BadRequest("Invalid quantity provided for decrement.");

            return await _productService.DecrementStockAsync(id, quantity);
        }

        [HttpPut("add-to-stock/{id}/{quantity}")]
        public async Task<IActionResult> IncrementStock(int id, int quantity)
        {
            if (id == null || quantity <= 0)
            {
                return BadRequest("Invalid quantity provided for increment.");
            }

            return await _productService.IncreaseStockAsync(id, quantity);
        }

        [HttpDelete("id")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id);

            if (result)
            {
                return Ok($"Product {id} deleted successfully.");
            }

            return NotFound($"Product with ID {id} not found.");
        }
    }
}



