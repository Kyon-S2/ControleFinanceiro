using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ControleFinanceiro.Application.DTOs;
using ControleFinanceiro.Application.Services;

namespace ControleFinanceiro.Api.Controllers;

[ApiController, Route("api/[controller]"), Authorize]
public class GastosController(IGastoService service) : ControllerBase
{
    private int UsuarioId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> Get()
        => Ok(await service.ObterTodosAsync(UsuarioId));

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] GastoRequest request)
    {
        var result = await service.CriarAsync(UsuarioId, request);
        return Created($"api/gastos/{result.Id}", result);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(int id, [FromBody] GastoRequest request)
    {
        try
        {
            var result = await service.AtualizarAsync(UsuarioId, id, request);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await service.RemoverAsync(UsuarioId, id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}

