using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Moq;
using ProductApi.Data.Context;
using ProductApi.Data.Repositories;
using ProductApi.Models.Entity;

namespace ProductApi.Tests.Data
{
    public class ProductRepositoryTests
    {
        private readonly Mock<ILogger<ProductRepository>> _mockLogger;
        private readonly ProductRepository _repository;
        private readonly AppDbContext _context;

        public ProductRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new AppDbContext(options);

            // Clear the database before each test
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _mockLogger = new Mock<ILogger<ProductRepository>>();
            _repository = new ProductRepository(_context, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnProducts()
        {
            // Arrange
            var product1 = new Product { Id = 1, Name = "Test Product 1", Price = 10.0 };
            var product2 = new Product { Id = 2, Name = "Test Product 2", Price = 20.0 };
            await _context.Products.AddRangeAsync(product1, product2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Any();
            result.Should().Contain(p => p.Name == "Test Product 1");
            result.Should().Contain(p => p.Name == "Test Product 2");
        }

        [Theory]
        [InlineData(1, "Product 1", 10.0)]
        [InlineData(2, "Product 2", 15.5)]
        [InlineData(3, "Product 3", 25.0)]
        public async Task GetByIdAsync_ShouldReturnProduct_WhenProductExists(int id, string name, double price)
        {
            // Arrange
            var product = new Product { Id = id, Name = name, Price = price };

            // Manually save the product first to ensure SaveChangesAsync is called and Id is generated.
            await _repository.AddAsync(product);

            // Act
            var result = await _repository.GetByIdAsync(product.Id); // Use the actual Id after saving

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(name);
            result.Price.Should().Be(price);
        }

        // Test for non-existent product
        [Theory]
        [InlineData(99)]
        [InlineData(100)]
        public async Task GetByIdAsync_ShouldReturnNull_WhenProductDoesNotExist(int id)
        {
            // Act
            var result = await _repository.GetByIdAsync(id);

            // Assert
            result.Should().BeNull();
        }

        // Testing adding products with varying price and name
        [Theory]
        [InlineData("Product 1", 10.0)]
        [InlineData("Product 2", 15.5)]
        [InlineData("Product 3", 25.0)]
        public async Task AddAsync_ShouldAddProduct_WithValidData(string name, double price)
        {
            // Arrange
            var product = new Product { Name = name, Price = price };

            // Act
            await _repository.AddAsync(product);

            // Assert
            var addedProduct = await _context.Products.FindAsync(product.Id);
            addedProduct.Should().NotBeNull();
            addedProduct.Name.Should().Be(name);
            addedProduct.Price.Should().Be(price);
        }

        // Testing for multiple products deletion
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task DeleteAsync_ShouldRemoveProduct(int id)
        {
            // Arrange
            var product = new Product { Id = id, Name = $"Product {id}", Price = 10.0 + id };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteAsync(id);

            // Assert
            var deletedProduct = await _context.Products.FindAsync(id);
            deletedProduct.Should().BeNull();
        }
    }
}
