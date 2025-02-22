using System.ComponentModel.DataAnnotations;

namespace ProductApi.Models.Entity
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }

        public Product()
        {
            Id = GenerateUniqueProductId();
        }

        private int GenerateUniqueProductId()
        {
            Random random = new Random();
            return random.Next(100000, 999999);
        }
    }
}
