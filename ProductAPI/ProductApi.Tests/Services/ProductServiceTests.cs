using Moq;
using Xunit;
using ProductApi.Services;
using ProductApi.Data.Repositories;
using ProductApi.Models.DTO;
using ProductApi.Models.Entity;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
namespace ProductApi.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _repositoryMock;
        private readonly Mock<ILogger<ProductService>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _repositoryMock = new Mock<IProductRepository>();
            _loggerMock = new Mock<ILogger<ProductService>>();
            _mapperMock = new Mock<IMapper>();
            _service = new ProductService(_repositoryMock.Object, _loggerMock.Object, _mapperMock.Object);
        }

        #region GetAllAsync

        [Fact]
        public async Task GetAllAsync_ReturnsAllProducts_WhenProductsExist()
        {
            // Arrange
            var products = new List<Product> { new Product { Id = 1, Name = "Product 1" }, new Product { Id = 2, Name = "Product 2" } };
            _repositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(products);
            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<Product>>(It.IsAny<IEnumerable<Product>>())).Returns(products);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllAsync_ThrowsException_WhenErrorOccurs()
        {
            // Arrange
            _repositoryMock.Setup(repo => repo.GetAllAsync()).ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.GetAllAsync());
            Assert.Equal("An error occurred while retrieving products.", ex.Message);
        }

        #endregion

        #region GetByIdAsync

        [Fact]
        public async Task GetByIdAsync_ReturnsProduct_WhenProductExists()
        {
            // Arrange
            var productId = 1;
            var product = new Product { Id = productId, Name = "Product 1" };
            var productDto = new ProductDto { Id = productId, Name = "Product 1" };

            _repositoryMock.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);
            _mapperMock.Setup(mapper => mapper.Map<ProductDto>(product)).Returns(productDto);

            // Act
            var result = await _service.GetByIdAsync(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenProductNotFound()
        {
            // Arrange
            var productId = 1;
            _repositoryMock.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync((Product)null);

            // Act
            var result = await _service.GetByIdAsync(productId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_ThrowsException_WhenErrorOccurs()
        {
            // Arrange
            var productId = 1;
            _repositoryMock.Setup(repo => repo.GetByIdAsync(productId)).ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.GetByIdAsync(productId));
            Assert.Equal($"An error occurred while retrieving product {productId}.", ex.Message);
        }

        #endregion

        #region CreateAsync

        [Fact]
        public async Task CreateAsync_ReturnsProductDto_WhenProductCreated()
        {
            // Arrange
            var createProductDto = new CreateProductDto { Name = "New Product", Price = 100 };
            var product = new Product { Id = 1, Name = "New Product", Price = 100 };
            var productDto = new ProductDto { Id = 1, Name = "New Product", Price = 100 };

            _mapperMock.Setup(mapper => mapper.Map<Product>(createProductDto)).Returns(product);
            _repositoryMock.Setup(repo => repo.AddAsync(product)).Returns(Task.CompletedTask);
            _mapperMock.Setup(mapper => mapper.Map<ProductDto>(product)).Returns(productDto);

            // Act
            var result = await _service.CreateAsync(createProductDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Product", result.Name);
        }

        [Fact]
        public async Task CreateAsync_ThrowsException_WhenErrorOccurs()
        {
            // Arrange
            var createProductDto = new CreateProductDto { Name = "New Product", Price = 100 };
            _repositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Product>())).ThrowsAsync(new Exception("Error"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.CreateAsync(createProductDto));
            Assert.Equal("An error occurred while creating the product.", ex.Message);
        }

        #endregion

        #region UpdateAsync

        [Fact]
        public async Task UpdateAsync_UpdatesProduct_WhenProductExists()
        {
            // Arrange
            var productId = 1;
            var updatedProduct = new UpdateProductDto { Name = "Updated Product" };
            _repositoryMock.Setup(repo => repo.ExistsAsync(productId)).ReturnsAsync(true);
            _mapperMock.Setup(mapper => mapper.Map<Product>(updatedProduct));
            var product = _mapperMock.Object.Map<Product>(updatedProduct);
            _repositoryMock.Setup(repo => repo.UpdateAsync(product)).Returns(Task.CompletedTask);

            // Act
            await _service.UpdateAsync(productId, updatedProduct);

            // Assert
            _repositoryMock.Verify(repo => repo.UpdateAsync(product), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ThrowsException_WhenProductNotFound()
        {
            // Arrange
            var productId = 1;
            var updatedProduct = new UpdateProductDto { Name = "Updated Product" };
            _repositoryMock.Setup(repo => repo.ExistsAsync(productId)).ReturnsAsync(false);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(productId, updatedProduct));
            Assert.Equal($"Product with ID {productId} not found.", ex.Message);
        }

        [Fact]
        public async Task UpdateAsync_ThrowsException_WhenErrorOccurs()
        {
            // Arrange
            var productId = 1;
            var updatedProduct = new UpdateProductDto { Name = "Updated Product" };
            _repositoryMock.Setup(repo => repo.ExistsAsync(productId)).ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.UpdateAsync(productId, updatedProduct));
            Assert.Equal($"An error occurred while updating product {productId}.", ex.Message);
        }

        #endregion

        #region DeleteAsync

        [Fact]
        public async Task DeleteAsync_DeletesProduct_WhenProductExists()
        {
            // Arrange
            var productId = 1;
            _repositoryMock.Setup(repo => repo.ExistsAsync(productId)).ReturnsAsync(true);
            _repositoryMock.Setup(repo => repo.DeleteAsync(productId)).Returns(Task.CompletedTask);

            // Act
            await _service.DeleteAsync(productId);

            // Assert
            _repositoryMock.Verify(repo => repo.DeleteAsync(productId), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ThrowsException_WhenProductNotFound()
        {
            // Arrange
            var productId = 1;
            _repositoryMock.Setup(repo => repo.ExistsAsync(productId)).ReturnsAsync(false);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync(productId));
            Assert.Equal($"Product with ID {productId} not found.", ex.Message);
        }

        [Fact]
        public async Task DeleteAsync_ThrowsException_WhenErrorOccurs()
        {
            // Arrange
            var productId = 1;
            _repositoryMock.Setup(repo => repo.ExistsAsync(productId)).ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.DeleteAsync(productId));
            Assert.Equal($"An error occurred while deleting product {productId}.", ex.Message);
        }

        #endregion
    }
}
