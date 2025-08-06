using Microsoft.EntityFrameworkCore;
using Products.Application.Dto;
using Products.Application.Utilities;
using Products.Domain;
using Products.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Products.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDbContext _dbContext;

        public ProductRepository(ProductDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _dbContext.Products.FindAsync(id);
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _dbContext.Products.ToListAsync();
        }

        public async Task AddAsync(Product dto)
        {
            //var product = new Product
            //{
            //    ProductId = ProductIdGenerator.GenerateProductId(),
            //    Name = dto.Name,
            //    Price = dto.Price,
            //    AvailableStock = dto.AvailableStock,
            //    Description = dto.Description
            //};

            await _dbContext.Products.AddAsync(dto);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await GetByIdAsync(id);
            if (product != null)
            {
                _dbContext.Products.Remove(product);
                await _dbContext.SaveChangesAsync();
            }
        }

        // Custom methods for stock management
        public async Task<bool> DecrementStockAsync(int productId, int quantity)
        {
            var product = await GetByIdAsync(productId);
            if (product != null && product.AvailableStock >= quantity)
            {
                product.AvailableStock -= quantity;
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> IncreaseStockAsync(int productId, int quantity)
        {
            var product = await GetByIdAsync(productId);
            if (product != null)
            {
                product.AvailableStock += quantity;
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
