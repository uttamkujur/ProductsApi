using Microsoft.AspNetCore.Mvc;
using Products.Application.Dto;
using Products.Domain;
using Products.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Products.Application
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<ActionResult<ProductDto>> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return new NotFoundResult();

            return new OkObjectResult( new ProductDto
            {
                Id = product.Id,
                Brand = product.Brand,
                Model = product.Model,
                Name = product.Name,
                Price = product.Price,
                AvailableStock = product.AvailableStock
            });
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Brand = p.Brand,
                Model = p.Model,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                AvailableStock = p.AvailableStock
            }).ToList();
        }

        public async Task<bool> AddProductAsync(CreateProductDto dto)
        {
            var product = new Product
            {
                Brand = dto.Brand,
                Model = dto.Model,
                Name = dto.Name,
                Price = dto.Price,
                AvailableStock = dto.AvailableStock,
                Description = dto.Description
            };
            await _productRepository.AddAsync(product);
            return true;
        }

        public async Task<bool> UpdateProductAsync(UpdateProductDto dto)
        {
            var product = await _productRepository.GetByIdAsync(dto.Id);
            if (product == null)
                return false;
            product.Brand = dto.Brand;
            product.Model = dto.Model;
            product.Name = dto.Name;
            product.Price = dto.Price;
            product.AvailableStock = dto.AvailableStock;

            await _productRepository.UpdateAsync(product);
            return true;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return false;

            product.IsActive = false; // Soft delete
            await _productRepository.UpdateAsync(product);
            return true;
        }

        public async Task<IActionResult> DecrementStockAsync(int id, int quantity)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
                return new NotFoundObjectResult($"Product with ID {id} not found.");

            if (product.AvailableStock < quantity)
                return new BadRequestObjectResult(
                    $"Insufficient stock. Available: {product.AvailableStock}, Requested: {quantity}."
                );

            //product.AvailableStock -= quantity;
            //await _productRepository.SaveAsync();

            //return new OkObjectResult($"Stock for product {id} successfully decremented by {quantity}.");

            await _productRepository.DecrementStockAsync(id, quantity);
            return new OkObjectResult($"Stock for product {id} successfully decremented by {quantity}.");
        }

        public async Task<IActionResult> IncreaseStockAsync(int id, int quantity)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
                return new NotFoundObjectResult($"Product with ID {id} not found.");
            
            await _productRepository.IncreaseStockAsync(id, quantity);
            return new OkObjectResult($"Stock for product {id} successfully incremented by {quantity}.");
        }
    }
}

