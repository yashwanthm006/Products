using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductApi.Data.Context;
using ProductApi.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductApi.Data.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<StockRepository> _logger;

        public StockRepository(AppDbContext context, ILogger<StockRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Stock?> GetStockByProductIdAsync(int productId)
        {
            try
            {
                _logger.LogInformation($"Fetching stock for product ID: {productId}");

                var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.ProductId == productId);
                return stock ?? null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching stock for product ID: {productId}");
                throw new Exception($"Error retrieving stock for product ID {productId}.", ex);
            }
        }

        public async Task UpdateStockAsync(int productId, int quantity)
        {
            try
            {
                _logger.LogInformation($"Updating stock for product ID: {productId}, Quantity: {quantity}");

                var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.ProductId == productId);

                if (stock == null)
                {
                    _logger.LogWarning($"Stock record not found for product ID: {productId}");
                    throw new KeyNotFoundException($"Stock record not found for product ID {productId}.");
                }

                stock.Quantity += quantity;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Stock updated successfully for product ID: {productId}, New Quantity: {stock.Quantity}");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"Concurrency error while updating stock for product ID: {productId}");
                throw new Exception("Concurrency error occurred while updating stock.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating stock for product ID: {productId}");
                throw new Exception($"Error updating stock for product ID {productId}.", ex);
            }
        }
    }
}
