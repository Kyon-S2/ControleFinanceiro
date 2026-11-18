using Microsoft.AspNetCore.Mvc;
using ControleFinanceiro.Application.Exceptions;

namespace ControleFinanceiro.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);

            if (context.Response.StatusCode == StatusCodes.Status404NotFound && !context.Response.HasStarted && context.Response.ContentType == null)
            {
                context.Response.ContentType = "application/problem+json";
                var problem = new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    Title = "Não Encontrado",
                    Status = StatusCodes.Status404NotFound,
                    Detail = $"O recurso '{context.Request.Path}' não foi encontrado.",
                    Instance = context.Request.Path
                };
                await context.Response.WriteAsJsonAsync(problem);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro capturado no middleware de exceções: {Path}", context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        if (context.Response.HasStarted)
            return;

        int statusCode;
        string title;
        string detail;
        string type;

        switch (ex)
        {
            case NaoEncontradoException nfe:
                statusCode = StatusCodes.Status404NotFound;
                title = "Não Encontrado";
                detail = nfe.Message;
                type = "https://tools.ietf.org/html/rfc7231#section-6.5.4";
                break;

            case ValidacaoException ve:
                statusCode = StatusCodes.Status422UnprocessableEntity;
                title = "Erro de Validação";
                detail = ve.Message;
                type = "https://tools.ietf.org/html/rfc4918#section-11.2";
                break;

            case ConflitoException ce:
                statusCode = StatusCodes.Status409Conflict;
                title = "Conflito";
                detail = ce.Message;
                type = "https://tools.ietf.org/html/rfc7231#section-6.5.8";
                break;

            case CredenciaisInvalidasException cie:
                statusCode = StatusCodes.Status401Unauthorized;
                title = "Não Autorizado";
                detail = cie.Message;
                type = "https://tools.ietf.org/html/rfc7235#section-3.1";
                break;

            case ProibidoException pe:
                statusCode = StatusCodes.Status403Forbidden;
                title = "Acesso Proibido";
                detail = pe.Message;
                type = "https://tools.ietf.org/html/rfc7231#section-6.5.3";
                break;

            case UnauthorizedAccessException uae:
                statusCode = StatusCodes.Status401Unauthorized;
                title = "Não Autorizado";
                detail = uae.Message;
                type = "https://tools.ietf.org/html/rfc7235#section-3.1";
                break;

            default:
                statusCode = StatusCodes.Status500InternalServerError;
                title = "Erro Interno do Servidor";
                detail = "Ocorreu um erro interno inesperado.";
                type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
                break;
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Type = type,
            Title = title,
            Status = statusCode,
            Detail = detail,
            Instance = context.Request.Path
        };

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}
