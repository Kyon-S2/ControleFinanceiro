using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ControleFinanceiro.Api.Extensions;
using ControleFinanceiro.Application.DTOs;
using ControleFinanceiro.Application.Interfaces;

namespace ControleFinanceiro.Api.Controllers;

/// <summary>
/// Gerenciamento de despesas e gastos do usuário.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class GastosController(IGastoService service) : ControllerBase
{
    /// <summary>
    /// Lista os gastos do usuário com filtros opcionais.
    /// </summary>
    /// <param name="nome">Filtro por nome</param>
    /// <param name="categoriaId">Filtro por ID da categoria</param>
    /// <param name="mes">Filtro por mês no formato yyyy-MM</param>
    [HttpGet]
    [ProducesResponseType(typeof(List<GastoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? nome,
        [FromQuery] int? categoriaId,
        [FromQuery] string? mes)
    {
        var list = await service.ObterTodosAsync(User.GetUsuarioId(), nome, categoriaId, mes);
        return Ok(list);
    }

    /// <summary>
    /// Obtém os detalhes de um gasto pelo ID.
    /// </summary>
    /// <param name="id">ID do gasto</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GastoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var gasto = await service.ObterPorIdAsync(User.GetUsuarioId(), id);
        return Ok(gasto);
    }

    /// <summary>
    /// Cadastra um novo gasto para o usuário logado.
    /// </summary>
    /// <param name="request">Dados do gasto</param>
    [HttpPost]
    [ProducesResponseType(typeof(GastoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] GastoRequest request)
    {
        var criado = await service.CriarAsync(User.GetUsuarioId(), request);
        return CreatedAtAction(nameof(GetById), new { id = criado.Id }, criado);
    }

    /// <summary>
    /// Atualiza um gasto existente do usuário logado.
    /// </summary>
    /// <param name="id">ID do gasto</param>
    /// <param name="request">Novos dados do gasto</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(GastoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(int id, [FromBody] GastoRequest request)
    {
        var atualizado = await service.AtualizarAsync(User.GetUsuarioId(), id, request);
        return Ok(atualizado);
    }

    /// <summary>
    /// Remove um gasto do usuário logado.
    /// </summary>
    /// <param name="id">ID do gasto</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await service.RemoverAsync(User.GetUsuarioId(), id);
        return NoContent();
    }
}
