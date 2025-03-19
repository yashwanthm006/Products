using Moq;
using Microsoft.Extensions.Logging;
using ProductApi.Models.Entity;
using ProductApi.Data.Repositories;
using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using ProductApi.Services;

namespace ProductApi.Tests.Services
{
    public class StockServiceTests
    {
        private readonly Mock<IProductRepository> _mockRepository;
        private readonly Mock<ILogger<StockService>> _mockLogger;
        private readonly StockService _service;

        public StockServiceTests()
        {
            _mockRepository = new Mock<IProductRepository>();
            _mockLogger = new Mock<ILogger<StockService>>();
            _service = new StockService(_mockRepository.Object, _mockLogger.Object);
        }

        #region DecrementStockAsync Tests

        [Fact]
        public async Task DecrementStockAsync_ShouldDecreaseStock_WhenProductExistsAndEnoughStock()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Product 1", Stock = 10 };
            _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(product);
            _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

            // Act
            await _service.DecrementStockAsync(1, 5);

            // Assert
            product.Stock.Should().Be(5); // Ensure stock is decremented correctly
            _mockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Product>()), Times.Once); // Ensure UpdateAsync was called
        }

        [Fact]
        public async Task DecrementStockAsync_ShouldThrowKeyNotFoundException_WhenProductNotFound()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Product)null);

            // Act
            Func<Task> act = async () => await _service.DecrementStockAsync(1, 5);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Product with ID 1 not found.");
        }

        [Fact]
        public async Task DecrementStockAsync_ShouldThrowInvalidOperationException_WhenNotEnoughStock()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Product 1", Stock = 5 };
            _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(product);

            // Act
            Func<Task> act = async () => await _service.DecrementStockAsync(1, 10);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Not enough stock for product ID 1.");
        }

        #endregion

        #region AddToStockAsync Tests

        [Fact]
        public async Task AddToStockAsync_ShouldIncreaseStock_WhenProductExists()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Product 1", Stock = 5 };
            _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(product);
            _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

            // Act
            await _service.AddToStockAsync(1, 5);

            // Assert
            product.Stock.Should().Be(10); // Ensure stock is increased correctly
            _mockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Product>()), Times.Once); // Ensure UpdateAsync was called
        }

        [Fact]
        public async Task AddToStockAsync_ShouldThrowKeyNotFoundException_WhenProductNotFound()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Product)null);

            // Act
            Func<Task> act = async () => await _service.AddToStockAsync(1, 5);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Product with ID 1 not found.");
        }

        #endregion

        #region DecrementStockAsync and AddToStockAsync Theory Tests

        // Test multiple quantities and stock updates with Theory
        [Theory]
        [InlineData(1, 5, 10, 5)] // Decrease stock of 10 by 5
        [InlineData(2, 3, 15, 12)] // Decrease stock of 15 by 3
        [InlineData(3, 0, 10, 10)] // Decrease stock by 0 (no change)
        public async Task DecrementStockAsync_ShouldWorkCorrectly_ForMultipleCases(int productId, int decrementQty, int initialStock, int expectedStock)
        {
            // Arrange
            var product = new Product { Id = productId, Name = $"Product {productId}", Stock = initialStock };
            _mockRepository.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);
            _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

            // Act
            await _service.DecrementStockAsync(productId, decrementQty);

            // Assert
            product.Stock.Should().Be(expectedStock); // Ensure stock is decremented correctly
            _mockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Product>()), Times.Once); // Ensure UpdateAsync was called
        }

        // Test multiple quantities and stock updates for AddToStockAsync
        [Theory]
        [InlineData(1, 5, 10, 15)] // Add stock of 5 to product with initial stock of 10
        [InlineData(2, 3, 15, 18)] // Add stock of 3 to product with initial stock of 15
        [InlineData(3, 0, 10, 10)] // Add 0 stock (no change)
        public async Task AddToStockAsync_ShouldWorkCorrectly_ForMultipleCases(int productId, int addQty, int initialStock, int expectedStock)
        {
            // Arrange
            var product = new Product { Id = productId, Name = $"Product {productId}", Stock = initialStock };
            _mockRepository.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);
            _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

            // Act
            await _service.AddToStockAsync(productId, addQty);

            // Assert
            product.Stock.Should().Be(expectedStock); // Ensure stock is added correctly
            _mockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Product>()), Times.Once); // Ensure UpdateAsync was called
        }

        #endregion
    }
}
