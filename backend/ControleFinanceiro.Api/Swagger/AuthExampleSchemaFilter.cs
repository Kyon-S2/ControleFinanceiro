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
        else if (context.Type == typeof(GoogleAuthRequest))
        {
            schema.Example = new OpenApiObject
            {
                ["idToken"] = new OpenApiString("eyJhbGciOiJSUzI1NiIsImtpZCI6ImFiY2RlZiJ9.eyJpc3MiOiJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20iLCJzdWIiOiIxMjM0NTY3ODkwIiwiZW1haWwiOiJ1c3VhcmlvQGV4ZW1wbG8uY29tIiwiZW1haWxfdmVyaWZpZWQiOnRydWV9...")
            };
        }
    }
}

