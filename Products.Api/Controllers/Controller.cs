using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Products.Application;
using Products.Application.Dto;
using Products.Domain;
using Products.Infrastructure;
using System.Collections.Generic;


namespace Products.Api
{
    [ApiController]
    [Route("api/[[controller]]")]

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
        [Route("{{id}}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }


        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(CreateProductDto product)
        {
            //_context.Products.Add(product);
            //await _context.SaveChangesAsync();
            //return CreatedAtAction("GetProduct", new { id = product.Id }, product);

            if (ModelState.IsValid)
            {
                await _productService.AddProductAsync(product);
                return CreatedAtAction(nameof(GetProduct), product);
            }

            return BadRequest(ModelState);
        }


        //PUT: api/Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct([FromQuery] int id, UpdateProductDto product)
        {
            if (id != product.Id)
                return BadRequest();

            var result = await _productService.UpdateProductAsync(product);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPut("{id}/decrement")]
        //[HttpPut("decrement-stock")]
        public async Task<IActionResult> DecrementStock([FromBody] DecrementStockDto dto)
        {
            if (dto == null || dto.Quantity <= 0)
            {
                return BadRequest("Invalid quantity provided for decrement.");
            }

            // Attempt to decrement the stock for the product
            var result = await _productService.DecrementStockAsync(dto.Id, dto.Quantity);

            if (result)
            {
                return Ok($"Stock for product {dto.Id} successfully decremented by {dto.Quantity}.");
            }

            return NotFound($"Product with ID {dto.Id} not found or insufficient stock.");
        }

        [HttpPut("{id}/increment")]
        //[HttpPut("increment-stock")]
        public async Task<IActionResult> IncrementStock([FromBody] IncrementStockDto dto)
        {
            if (dto == null || dto.Quantity <= 0)
            {
                return BadRequest("Invalid quantity provided for increment.");
            }

            // Attempt to increment the stock for the product
            var result = await _productService.IncreaseStockAsync(dto.Id, dto.Quantity);

            if (result)
            {
                return Ok($"Stock for product {dto.Id} successfully incremented by {dto.Quantity}.");
            }

            return NotFound($"Product with ID {dto.Id} not found.");
        }



        //DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();


            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}



