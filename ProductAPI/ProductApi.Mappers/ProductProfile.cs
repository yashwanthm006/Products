using ProductApi.Models.Entity;
using AutoMapper;
using ProductApi.Models.DTO;

namespace ProductApi.Mappers
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<CreateProductDto, Product>(); // For creating a product
            CreateMap<UpdateProductDto, Product>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null)); // For updating a product
            CreateMap<Product, ProductDto>(); // For returning a product response
        }
    }
}
