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
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(AppDbContext context, ILogger<ProductRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all products");
            try
            {
                return await _context.Products.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching all products. Exception : {ex.Message}");
                throw new Exception("Error fetching all products.", ex);
            }
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            _logger.LogInformation($"Fetching product with ID: {id}");
            try
            {
                var product = await _context.Products.FindAsync(id);
                return product ?? null; // Explicitly returning null if not found
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching product with ID {id}. Exception : {ex.Message}");
                throw new Exception($"Error fetching product with ID {id}.", ex);
            }
        }

        public async Task AddAsync(Product product)
        {
            _logger.LogInformation("Adding a new product");
            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding product. Exception : {ex.Message}");
                throw new Exception("Error adding product.", ex);
            }
        }

        public async Task UpdateAsync(Product product)
        {
            _logger.LogInformation($"Updating product with ID: {product.Id}");
            try
            {
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError($"Concurrency error while updating product. Exception : {ex.Message}");
                throw new Exception("Concurrency error while updating product.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating product. Exception : {ex.Message}");
                throw new Exception("Error updating product.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            _logger.LogInformation($"Deleting product with ID: {id}");
            try
            {
                var product = await GetByIdAsync(id);
                if (product != null)
                {
                    _context.Products.Remove(product);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting product with ID. Exception : {ex.Message}");
                throw new Exception($"Error deleting product with ID {id}.", ex);
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            try
            {
                return await _context.Products.AnyAsync(p => p.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking product existence. Exception : {ex.Message}");
                throw new Exception("Error checking product existence.", ex);
            }
        }
    }
}
