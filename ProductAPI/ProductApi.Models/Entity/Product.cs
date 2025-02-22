using ProductApi.Shared.Utils;
using System.ComponentModel.DataAnnotations;

namespace ProductApi.Models.Entity
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string ProductNumber { get; set; } = IdGenerator.GenerateUniqueId();
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
