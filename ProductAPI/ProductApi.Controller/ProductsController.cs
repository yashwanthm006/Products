using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductApi.Models.DTO;
using ProductApi.Models.Entity;
using ProductApi.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace ProductApi.Controller
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IStockService _stockService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, IStockService stockService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _stockService = stockService;
            _logger = logger;
        }

        /// <summary>
        /// Get all products.
        /// </summary>
        /// <returns>List of all products</returns>
        /// <response code="200">Returns the list of products</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [SwaggerOperation(Summary = "Returns the list of products")]
        [SwaggerResponse((int)HttpStatusCode.OK, type: typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, type: typeof(Exception))]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var products = await _productService.GetAllAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching the products. Error : {ex.Message}");
                return StatusCode(500, "An error occurred while fetching the products.");
            }
        }

        /// <summary>
        /// Get a product by its ID.
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>The product with the specified ID</returns>
        /// <response code="200">Returns the product</response>
        /// <response code="404">Product not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Gets the product with the specified ID")]
        [SwaggerResponse((int)HttpStatusCode.OK, type: typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, type: typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, type: typeof(Exception))]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);
                if (product == null)
                {
                    return NotFound($"Product with ID {id} not found.");
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching the product. Error : {ex.Message}");
                return StatusCode(500, "An error occurred while fetching the product.");
            }
        }

        /// <summary>
        /// Create a new product.
        /// </summary>
        /// <param name="createProductDto">The product details</param>
        /// <returns>The created product</returns>
        /// <response code="201">Product created successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [SwaggerOperation(Summary = "Creates the Product")]
        [SwaggerResponse((int)HttpStatusCode.Created, type: typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, type: typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, type: typeof(Exception))]
        public async Task<IActionResult> Create([FromBody] CreateProductDto createProductDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Automatically returns validation errors
            }

            try
            {
                var createdProduct = await _productService.CreateAsync(createProductDto);
                return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while creating the product. Error : {ex.Message}");
                return StatusCode(500, "An error occurred while creating the product.");
            }
        }

        /// <summary>
        /// Update an existing product.
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="product">The product details</param>
        /// <returns>No content (204) if successful</returns>
        /// <response code="204">Product updated successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="404">Product not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Updates the existing Product based on ProductId")]
        [SwaggerResponse((int)HttpStatusCode.Created, type: typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, type: typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, type: typeof(Exception))]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto updateProductDto)
        {
            try
            {
                var existingProduct = await _productService.GetByIdAsync(id);
                if (existingProduct == null)
                {
                    return NotFound($"Product with ID {id} not found.");
                }

                await _productService.UpdateAsync(id, updateProductDto);
                return NoContent(); // Indicates successful update
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError($"Product {id} Not Found. Error : {ex.Message}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while updating the product. Error : {ex.Message}");
                return StatusCode(500, "An error occurred while updating the product.");
            }
        }

        /// <summary>
        /// Delete a product by its ID.
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>No content (204) if successful</returns>
        /// <response code="204">Product deleted successfully</response>
        /// <response code="404">Product not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Deletes the product based on ProductID")]
        [SwaggerResponse((int)HttpStatusCode.OK, type: typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, type: typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, type: typeof(Exception))]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);
                if (product == null)
                {
                    return NotFound($"Product with ID {id} not found.");
                }

                await _productService.DeleteAsync(id);
                return NoContent(); // Indicates successful deletion
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while updating the product. Error : {ex.Message}");
                return StatusCode(500, "An error occurred while deleting the product.");
            }
        }

        /// <summary>
        /// Decrement the stock for a product.
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="quantity">Quantity to decrement</param>
        /// <returns>Ok response if successful</returns>
        /// <response code="200">Stock decremented successfully</response>
        /// <response code="400">Invalid quantity</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("decrement-stock/{id}/{quantity}")]
        [SwaggerOperation(Summary = "Increments the stock based on productId and returns OK Response")]
        [SwaggerResponse((int)HttpStatusCode.OK, type: typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, type: typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, type: typeof(Exception))]
        public async Task<IActionResult> DecrementStock(int id, int quantity)
        {
            if (quantity <= 0)
            {
                return BadRequest("Quantity must be greater than zero.");
            }

            try
            {
                await _stockService.DecrementStockAsync(id, quantity);
                return Ok(); // Successful stock decrement
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while decrementing the stock. Error : {ex.Message}");
                return StatusCode(500, "An error occurred while decrementing the stock.");
            }
        }

        /// <summary>
        /// Add to the stock for a product.
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <param name="quantity">Quantity to add</param>
        /// <returns>Ok response if successful</returns>
        /// <response code="200">Stock added successfully</response>
        /// <response code="400">Invalid quantity</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("add-to-stock/{id}/{quantity}")]
        [SwaggerOperation(Summary = "Decrements the stock based on productId and returns OK Response")]
        [SwaggerResponse((int)HttpStatusCode.OK, type: typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, type: typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, type: typeof(Exception))]
        public async Task<IActionResult> AddToStock(int id, int quantity)
        {
            if (quantity <= 0)
            {
                return BadRequest("Quantity must be greater than zero.");
            }

            try
            {
                await _stockService.AddToStockAsync(id, quantity);
                return Ok(); // Successful stock addition
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while incrementing the stock. Error : {ex.Message}");
                return StatusCode(500, "An error occurred while adding to the stock.");
            }
        }
    }
}
