using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductApi.Services
{
    public interface IStockService
    {
        Task DecrementStockAsync(int id, int quantity);
        Task AddToStockAsync(int id, int quantity);
    }
}
