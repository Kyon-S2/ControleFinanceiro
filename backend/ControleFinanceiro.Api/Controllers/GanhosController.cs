using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ControleFinanceiro.Api.Extensions;
using ControleFinanceiro.Application.DTOs;
using ControleFinanceiro.Application.Interfaces;

namespace ControleFinanceiro.Api.Controllers;

/// <summary>
/// Gerenciamento de ganhos e receitas do usuário.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class GanhosController(IGanhoService service) : ControllerBase
{
    /// <summary>
    /// Lista os ganhos do usuário com filtros opcionais.
    /// </summary>
    /// <param name="nome">Filtro por nome</param>
    /// <param name="categoriaId">Filtro por ID da categoria</param>
    /// <param name="mes">Filtro por mês no formato yyyy-MM</param>
    [HttpGet]
    [ProducesResponseType(typeof(List<GanhoResponse>), StatusCodes.Status200OK)]
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
    /// Obtém os detalhes de um ganho pelo ID.
    /// </summary>
    /// <param name="id">ID do ganho</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GanhoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var ganho = await service.ObterPorIdAsync(User.GetUsuarioId(), id);
        return Ok(ganho);
    }

    /// <summary>
    /// Cadastra um novo ganho para o usuário logado.
    /// </summary>
    /// <param name="request">Dados do ganho</param>
    [HttpPost]
    [ProducesResponseType(typeof(GanhoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] GanhoRequest request)
    {
        var criado = await service.CriarAsync(User.GetUsuarioId(), request);
        return CreatedAtAction(nameof(GetById), new { id = criado.Id }, criado);
    }

    /// <summary>
    /// Atualiza um ganho existente do usuário logado.
    /// </summary>
    /// <param name="id">ID do ganho</param>
    /// <param name="request">Novos dados do ganho</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(GanhoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(int id, [FromBody] GanhoRequest request)
    {
        var atualizado = await service.AtualizarAsync(User.GetUsuarioId(), id, request);
        return Ok(atualizado);
    }

    /// <summary>
    /// Remove um ganho do usuário logado.
    /// </summary>
    /// <param name="id">ID do ganho</param>
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
