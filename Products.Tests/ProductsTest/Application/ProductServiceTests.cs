
using Xunit;
using Moq;
using Products.Application; 

using FluentAssertions;
using Products.Domain;
using Products.Domain.Repositories;
using Products.Application.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xunit.Sdk;

namespace Products.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _productService = new ProductService(_mockProductRepository.Object);
        }

        #region GetProductByIdAsync Tests

        [Fact]
        public async Task GetProductByIdAsync_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            _mockProductRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Product)null);

            // Act
            var result = await _productService.GetProductByIdAsync(1);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetProductByIdAsync_ReturnsProduct_WhenProductExists()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Product1", Brand = "Brand1", Model = "Model1", Price = 100, AvailableStock = 10 };
            _mockProductRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(product);

            // Act
            var result = await _productService.GetProductByIdAsync(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProduct = Assert.IsType<ProductDto>(okResult.Value);
            Assert.Equal(product.Id, returnedProduct.Id);
        }
        #endregion

        #region GetAllProducts Tests

        [Fact]
        public async Task GetAllProductsAsync_ReturnsListOfProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product1", Brand = "Brand1", Model = "Model1", Price = 100, AvailableStock = 10 },
                new Product { Id = 2, Name = "Product2", Brand = "Brand2", Model = "Model2", Price = 200, AvailableStock = 20 }
            };
            _mockProductRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(products);

            // Act
            var result = await _productService.GetAllProductsAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        #endregion

        #region AddProductAsync Tests

        [Fact]
        public async Task AddProductAsync_ReturnsTrue_WhenProductIsAdded()
        {
            // Arrange
            var createProductDto = new CreateProductDto
            {
                Brand = "Brand1",
                Model = "Model1",
                Name = "Product1",
                Price = 100,
                AvailableStock = 10,
                Description = "Description"
            };

            _mockProductRepository.Setup(repo => repo.AddAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

            // Act
            var result = await _productService.AddProductAsync(createProductDto);

            // Assert
            Assert.True(result);
        }

        #endregion

        #region UpdateProductAsync Tests

        [Fact]
        public async Task UpdateProductAsync_ReturnsFalse_WhenProductDoesNotExist()
        {
            // Arrange
            var updateProductDto = new UpdateProductDto { Id = 1, Name = "Updated Product" };
            _mockProductRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Product)null);

            // Act
            var result = await _productService.UpdateProductAsync(updateProductDto);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateProductAsync_ReturnsTrue_WhenProductIsUpdated()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Product1", Brand = "Brand1", Model = "Model1", Price = 100, AvailableStock = 10 };
            var updateProductDto = new UpdateProductDto { Id = 1, Name = "Updated Product", Brand = "Updated Brand", Model = "Updated Model", Price = 200, AvailableStock = 20 };

            _mockProductRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(product);
            _mockProductRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

            // Act
            var result = await _productService.UpdateProductAsync(updateProductDto);

            // Assert
            Assert.True(result);
            Assert.Equal("Updated Product", product.Name);
        }

        #endregion

        #region DeleteProductAsync Tests

        [Fact]
        public async Task DeleteProductAsync_ReturnsFalse_WhenProductDoesNotExist()
        {
            // Arrange
            _mockProductRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Product)null);

            // Act
            var result = await _productService.DeleteProductAsync(1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteProductAsync_ReturnsTrue_WhenProductIsDeleted()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Product1", Brand = "Brand1", Model = "Model1", Price = 100, AvailableStock = 10 };
            _mockProductRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(product);
            _mockProductRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

            // Act
            var result = await _productService.DeleteProductAsync(1);

            // Assert
            Assert.True(result);
            Assert.False(product.IsActive); // Soft delete
        }

        #endregion

        #region DecrementStockAsync Tests

        [Fact]
        public async Task DecrementStockAsync_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            _mockProductRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Product)null);

            // Act
            var result = await _productService.DecrementStockAsync(1, 5);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Product with ID 1 not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task DecrementStockAsync_ReturnsBadRequest_WhenInsufficientStock()
        {
            // Arrange
            var product = new Product { Id = 1, AvailableStock = 5 };
            _mockProductRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(product);

            // Act
            var result = await _productService.DecrementStockAsync(1, 10);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Insufficient stock. Available: 5, Requested: 10.", badRequestResult.Value);
        }

        [Fact]
        public async Task DecrementStockAsync_ReturnsOk_WhenStockIsDecremented()
        {
            // Arrange
            var product = new Product { Id = 1, AvailableStock = 10 };
            _mockProductRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(product);
            _mockProductRepository.Setup(repo => repo.DecrementStockAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);

            // Act
            var result = await _productService.DecrementStockAsync(1, 5);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Stock for product 1 successfully decremented by 5.", okResult.Value);
        }

        #endregion

        #region IncreaseStockAsync Tests

        [Fact]
        public async Task IncreaseStockAsync_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            _mockProductRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Product)null);

            // Act
            var result = await _productService.IncreaseStockAsync(1, 5);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Product with ID 1 not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task IncreaseStockAsync_ReturnsOk_WhenStockIsIncreased()
        {
            // Arrange
            var product = new Product { Id = 1, AvailableStock = 10 };
            _mockProductRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(product);
        }
        #endregion
    }
}
