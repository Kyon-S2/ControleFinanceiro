using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ControleFinanceiro.Api.Extensions;
using ControleFinanceiro.Application.DTOs;
using ControleFinanceiro.Application.Interfaces;

namespace ControleFinanceiro.Api.Controllers;

/// <summary>
/// Resumo e métricas financeiras consolidadas do usuário.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ResumoController(IResumoService service) : ControllerBase
{
    /// <summary>
    /// Retorna o resumo consolidado do mês (total de gastos, receitas, saldo, limite e status).
    /// </summary>
    /// <param name="mes">Mês de referência no formato yyyy-MM (padrão: mês atual)</param>
    [HttpGet]
    [ProducesResponseType(typeof(ResumoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Get([FromQuery] string? mes)
    {
        var resumo = await service.ObterResumoAsync(User.GetUsuarioId(), mes);
        return Ok(resumo);
    }
}
