﻿
namespace ProductApi.Models.DTO
{
    public class CreateProductDto
    {
        public string? Name { get; set; }

        public decimal Price { get; set; }

        public string? Description { get; set; }

        public int Stock { get; set; }
    }
}
