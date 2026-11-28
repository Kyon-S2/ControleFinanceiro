using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ControleFinanceiro.Application.DTOs;
using ControleFinanceiro.Application.Interfaces;

namespace ControleFinanceiro.Api.Controllers;

/// <summary>
/// Endpoints para cadastro e autenticação de usuários.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Cadastra um novo usuário com e-mail e senha.
    /// </summary>
    /// <remarks>
    /// Exemplo de requisição:
    ///
    ///     POST /api/auth/register
    ///     {
    ///        "email": "usuario@exemplo.com",
    ///        "senha": "SenhaForte123"
    ///     }
    ///
    /// </remarks>
    /// <param name="request">Credenciais de cadastro</param>
    /// <returns>Token JWT de acesso</returns>
    [HttpPost("register")]
    [EnableRateLimiting("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await authService.RegisterAsync(request);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>
    /// Autentica o usuário utilizando e-mail e senha.
    /// </summary>
    /// <remarks>
    /// Exemplo de requisição:
    ///
    ///     POST /api/auth/login
    ///     {
    ///        "email": "usuario@exemplo.com",
    ///        "senha": "SenhaForte123"
    ///     }
    ///
    /// </remarks>
    /// <param name="request">Credenciais de login</param>
    /// <returns>Token JWT de acesso</returns>
    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await authService.LoginAsync(request);
        return Ok(result);
    }

}
