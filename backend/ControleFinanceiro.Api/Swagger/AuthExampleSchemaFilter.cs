using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using ControleFinanceiro.Application.DTOs;

namespace ControleFinanceiro.Api.Swagger;

public class AuthExampleSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(RegisterRequest))
        {
            schema.Example = new OpenApiObject
            {
                ["email"] = new OpenApiString("usuario@exemplo.com"),
                ["senha"] = new OpenApiString("SenhaForte123")
            };
        }
        else if (context.Type == typeof(LoginRequest))
        {
            schema.Example = new OpenApiObject
            {
                ["email"] = new OpenApiString("usuario@exemplo.com"),
                ["senha"] = new OpenApiString("SenhaForte123")
            };
        }
    }
}

