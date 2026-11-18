using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ControleFinanceiro.Api.Extensions;
using ControleFinanceiro.Application.DTOs;
using ControleFinanceiro.Application.Interfaces;

namespace ControleFinanceiro.Api.Controllers;

/// <summary>
/// Configuração do limite mensal de gastos do usuário.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LimiteController(ILimiteService service) : ControllerBase
{
    /// <summary>
    /// Obtém o limite mensal configurado para o usuário logado.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(LimiteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Get()
    {
        var limite = await service.ObterAsync(User.GetUsuarioId());
        return Ok(limite);
    }

    /// <summary>
    /// Define ou atualiza o limite mensal de gastos do usuário logado.
    /// </summary>
    /// <param name="request">Novo valor do limite</param>
    [HttpPut]
    [ProducesResponseType(typeof(LimiteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Put([FromBody] LimiteRequest request)
    {
        var atualizado = await service.DefinirAsync(User.GetUsuarioId(), request);
        return Ok(atualizado);
    }
}
