using Microsoft.OpenApi.Models;
using ProductApi.Models.DTO;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ProductApi.API.Swagger
{
    public class IgnoreIdInPostRequest : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(CreateProductDto))
            {
                schema.Properties.Remove("id");
            }
        }
    }
}
