using Microsoft.Extensions.Logging;
using ProductApi.Data.Repositories;
using ProductApi.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductApi.Services
{

    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository repository, ILogger<ProductService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all products.");
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all products.");
                throw new Exception("An error occurred while retrieving products.", ex);
            }
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Fetching product with ID: {id}");
                var product = await _repository.GetByIdAsync(id);
                if (product == null)
                {
                    _logger.LogWarning($"Product with ID {id} not found.");
                }
                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching product with ID: {id}");
                throw new Exception($"An error occurred while retrieving product {id}.", ex);
            }
        }

        public async Task<Product> CreateAsync(Product product)
        {
            try
            {
                _logger.LogInformation("Creating new product.");
                await _repository.AddAsync(product);
                _logger.LogInformation($"Product created successfully with ID: {product.Id}");
                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product.");
                throw new Exception("An error occurred while creating the product.", ex);
            }
        }

        public async Task UpdateAsync(int id, Product product)
        {
            try
            {
                _logger.LogInformation($"Updating product with ID: {id}");
                if (await _repository.ExistsAsync(id))
                {
                    await _repository.UpdateAsync(product);
                    _logger.LogInformation($"Product with ID {id} updated successfully.");
                }
                else
                {
                    _logger.LogWarning($"Update failed. Product with ID {id} does not exist.");
                    throw new KeyNotFoundException($"Product with ID {id} not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating product with ID: {id}");
                throw new Exception($"An error occurred while updating product {id}.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Deleting product with ID: {id}");
                if (await _repository.ExistsAsync(id))
                {
                    await _repository.DeleteAsync(id);
                    _logger.LogInformation($"Product with ID {id} deleted successfully.");
                }
                else
                {
                    _logger.LogWarning($"Delete failed. Product with ID {id} does not exist.");
                    throw new KeyNotFoundException($"Product with ID {id} not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting product with ID: {id}");
                throw new Exception($"An error occurred while deleting product {id}.", ex);
            }
        } 
    }

}
