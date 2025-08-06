using Products.Application.Dto;
using Products.Application.Utilities;
using Products.Domain;
using Products.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return null; // or throw custom exception

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                AvailableStock = product.AvailableStock
            };
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                AvailableStock = p.AvailableStock
            }).ToList();
        }

        public async Task<bool> AddProductAsync(CreateProductDto dto)
        {
            var product = new Product
            {
                //ProductId = ProductIdGenerator.GenerateProductId(),
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

            product.Name = dto.Name;
            product.Price = dto.Price;
            product.AvailableStock = dto.AvailableStock;

            await _productRepository.UpdateAsync(product);
            return true;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            await _productRepository.DeleteAsync(id);
            return true;
        }

        public async Task<bool> DecrementStockAsync(int id, int quantity)
        {
            return await _productRepository.DecrementStockAsync(id, quantity);
        }

        public async Task<bool> IncreaseStockAsync(int id, int quantity)
        {
            return await _productRepository.IncreaseStockAsync(id, quantity);
        }
    }
}

