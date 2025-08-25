using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Products.Domain;
using Products.Domain.Repositories;


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
            var product =  await _dbContext.Products
                .Where(p => p.Id == id && p.IsActive).FirstOrDefaultAsync();

            if (product != null)
            {
                return product;
            }
            return null;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _dbContext.Products.Where(p=>p.IsActive.Equals(true)).ToListAsync();
        }

        public async Task AddAsync(Product dto)
        {
            try
            {
                await _dbContext.Products.AddAsync(dto);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Check if it's a unique constraint violation
                if (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601) // 2627 = Unique constraint violation
                {
                    throw new InvalidOperationException($"Product with Model '{dto.Model}' and Brand '{dto.Brand}' already exists.");
                }
                // Re-throw if it's not the error we expect
                throw;
            }
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

        //Custom methods for stock management
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

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
