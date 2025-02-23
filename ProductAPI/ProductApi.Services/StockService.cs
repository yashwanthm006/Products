using Microsoft.Extensions.Logging;
using ProductApi.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductApi.Services
{
    public class StockService : IStockService
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<StockService> _logger;

        public StockService(IProductRepository repository, ILogger<StockService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task DecrementStockAsync(int id, int quantity)
        {
            try
            {
                _logger.LogInformation($"Decreasing stock for product ID: {id} by {quantity}");
                var product = await _repository.GetByIdAsync(id);

                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with ID {id} not found.");
                }

                if (product.Stock < quantity)
                {
                    throw new InvalidOperationException($"Not enough stock for product ID {id}.");
                }

                product.Stock -= quantity;
                await _repository.UpdateAsync(product);
                _logger.LogInformation($"Stock decremented successfully for product ID: {id}. New stock: {product.Stock}");
            }
            catch(KeyNotFoundException  ex)
            {
                _logger.LogWarning($"Stock decrement failed. Product with ID {id} not found.");
                throw;
            }
            catch(InvalidOperationException e)
            {
                _logger.LogWarning($"Stock decrement failed. Not enough stock for product ID: {id}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error decrementing stock for product ID: {id}");
                throw new Exception($"An error occurred while decrementing stock for product {id}.", ex);
            }
        }

        public async Task AddToStockAsync(int id, int quantity)
        {
            try
            {
                
                var product = await _repository.GetByIdAsync(id);

                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with ID {id} not found.");
                }

                product.Stock += quantity;
                await _repository.UpdateAsync(product);
                _logger.LogInformation($"Stock added successfully for product ID: {id}. New stock: {product.Stock}");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogInformation($"Adding {quantity} to stock for product ID: {id}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding stock for product ID: {id}");
                throw new Exception($"An error occurred while adding stock for product {id}.", ex);
            }
        }
    }
}
