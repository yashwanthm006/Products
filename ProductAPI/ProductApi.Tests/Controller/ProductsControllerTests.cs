using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ProductApi.Controller;
using ProductApi.Models.DTO;
using ProductApi.Models.Entity;
using ProductApi.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductApi.Tests.Controller
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductService> _productServiceMock;
        private readonly Mock<IStockService> _stockServiceMock;
        private readonly Mock<ILogger<ProductsController>> _loggerMock;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _productServiceMock = new Mock<IProductService>();
            _stockServiceMock = new Mock<IStockService>();
            _loggerMock = new Mock<ILogger<ProductsController>>();
            _controller = new ProductsController(_productServiceMock.Object, _stockServiceMock.Object, _loggerMock.Object);
        }

        #region GetAll

        [Fact]
        public async Task GetAll_ReturnsOkResult_WhenProductsExist()
        {
            // Arrange
            IEnumerable products = new[] { new Product { Id = 1, Name = "Product 1" }, new Product { Id = 2, Name = "Product 2" } };
            _productServiceMock.Setup(service => service.GetAllAsync()).ReturnsAsync((IEnumerable<Product>)products);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProducts = Assert.IsType<Product[]>(okResult.Value);
            Assert.Equal(2, returnedProducts.Length);
        }

        [Fact]
        public async Task GetAll_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            _productServiceMock.Setup(service => service.GetAllAsync()).ThrowsAsync(new System.Exception("Database error"));

            // Act
            var result = await _controller.GetAll();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode); // Internal server error
        }

        #endregion

        #region GetById

        [Fact]
        public async Task GetById_ReturnsOkResult_WhenProductExists()
        {
            // Arrange
            var productId = 1;
            var productDto = new ProductDto { Id = productId, Name = "Product 1" };
            _productServiceMock.Setup(service => service.GetByIdAsync(productId)).ReturnsAsync(productDto);

            // Act
            var result = await _controller.GetById(productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProduct = Assert.IsType<ProductDto>(okResult.Value);
            Assert.Equal(productId, returnedProduct.Id);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = 1;
            _productServiceMock.Setup(service => service.GetByIdAsync(productId)).ReturnsAsync((ProductDto)null);

            // Act
            var result = await _controller.GetById(productId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        #endregion

        #region Create

        [Fact]
        public async Task Create_ReturnsCreatedAtActionResult_WhenProductIsCreated()
        {
            // Arrange
            var createProductDto = new CreateProductDto { Name = "New Product", Price = 100 };
            var createdProduct = new ProductDto { Id = 1, Name = "New Product", Price = 100 };
            _productServiceMock.Setup(service => service.CreateAsync(createProductDto)).ReturnsAsync(createdProduct);

            // Act
            var result = await _controller.Create(createProductDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetById", createdAtActionResult.ActionName);
            var returnedProduct = Assert.IsType<ProductDto>(createdAtActionResult.Value);
            Assert.Equal("New Product", returnedProduct.Name);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            var createProductDto = new CreateProductDto { Name = "New Product", Price = 100 };
            _productServiceMock.Setup(service => service.CreateAsync(createProductDto)).ThrowsAsync(new Exception("An error occurred while creating the product"));

            // Act
            var result = await _controller.Create(createProductDto);

            // Assert
            var exception = Assert.IsType<ObjectResult>(result);
            Assert.Contains("An error occurred while creating the product", exception.Value.ToString()); // Bad request
        }

        #endregion

        #region Update

        [Fact]
        public async Task Update_ReturnsNoContent_WhenProductIsUpdated()
        {
            // Arrange
            var productId = 1;
            var updatedProduct = new UpdateProductDto { Name = "Updated Product" };

            // Set up the mock to simulate successful update (no exceptions thrown)
            _productServiceMock.Setup(service => service.GetByIdAsync(productId))
                .ReturnsAsync(new ProductDto());
            _productServiceMock.Setup(service => service.UpdateAsync(productId, updatedProduct))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(productId, updatedProduct);

            // Assert
            // Verify that the result is a NoContentResult (HTTP 204)
            Assert.IsType<NoContentResult>(result);
        }


        [Fact]
        public async Task Update_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = 1;
            var updatedProduct = new UpdateProductDto { Name = "Updated Product" };

            // Mock the UpdateAsync method to throw a KeyNotFoundException
            _productServiceMock.Setup(service => service.UpdateAsync(productId, updatedProduct))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.Update(productId, updatedProduct);

            // Assert
            // Verify that the result is of type NotFoundResult
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

            // Verify that the error message is as expected
            Assert.Equal("Product with ID 1 not found.", notFoundResult.Value);
        }


        #endregion

        #region Delete

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenProductIsDeleted()
        {
            // Arrange
            _productServiceMock.Setup(service => service.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new ProductDto());
            _productServiceMock.Setup(service => service.DeleteAsync(It.IsAny<int>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(It.IsAny<int>());

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = 1;
            _productServiceMock.Setup(service => service.DeleteAsync(productId)).ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.Delete(productId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        #endregion

        #region Stock Methods

        [Theory]
        [InlineData(1, 10)]
        public async Task DecrementStock_ReturnsOk_WhenStockDecreased(int productId, int quantity)
        {
            // Arrange
            _stockServiceMock.Setup(service => service.DecrementStockAsync(productId, quantity)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DecrementStock(productId, quantity);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Theory]
        [InlineData(1, 10)]
        public async Task AddToStock_ReturnsOk_WhenStockIncreased(int productId, int quantity)
        {
            // Arrange
            _stockServiceMock.Setup(service => service.AddToStockAsync(productId, quantity)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddToStock(productId, quantity);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        #endregion
    }
}
