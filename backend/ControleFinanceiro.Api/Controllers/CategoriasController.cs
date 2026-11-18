using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ControleFinanceiro.Api.Extensions;
using ControleFinanceiro.Application.DTOs;
using ControleFinanceiro.Application.Interfaces;

namespace ControleFinanceiro.Api.Controllers;

/// <summary>
/// Gerenciamento de categorias do usuário logado.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategoriasController(ICategoriaService service) : ControllerBase
{
    /// <summary>
    /// Lista todas as categorias pertencentes ao usuário logado.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<CategoriaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        var list = await service.ObterTodasAsync(User.GetUsuarioId());
        return Ok(list);
    }

    /// <summary>
    /// Obtém uma categoria específica pelo ID.
    /// </summary>
    /// <param name="id">ID da categoria</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CategoriaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var categoria = await service.ObterPorIdAsync(User.GetUsuarioId(), id);
        return Ok(categoria);
    }

    /// <summary>
    /// Cadastra uma nova categoria para o usuário logado.
    /// </summary>
    /// <param name="request">Dados da categoria</param>
    [HttpPost]
    [ProducesResponseType(typeof(CategoriaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] CategoriaRequest request)
    {
        var criada = await service.CriarAsync(User.GetUsuarioId(), request);
        return CreatedAtAction(nameof(GetById), new { id = criada.Id }, criada);
    }

    /// <summary>
    /// Atualiza uma categoria existente do usuário logado.
    /// </summary>
    /// <param name="id">ID da categoria</param>
    /// <param name="request">Novos dados da categoria</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(CategoriaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(int id, [FromBody] CategoriaRequest request)
    {
        var atualizada = await service.AtualizarAsync(User.GetUsuarioId(), id, request);
        return Ok(atualizada);
    }

    /// <summary>
    /// Remove uma categoria do usuário logado.
    /// </summary>
    /// <param name="id">ID da categoria</param>
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
