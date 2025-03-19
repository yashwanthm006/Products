using ProductApi.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductApi.Data.Repositories
{
    public interface IStockRepository
    {
        Task<Stock?> GetStockByProductIdAsync(int productId);
        Task UpdateStockAsync(int productId, int quantity);
    }
}
