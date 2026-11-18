using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ControleFinanceiro.Application.DTOs;
using ControleFinanceiro.Application.Services;

namespace ControleFinanceiro.Api.Controllers;

[ApiController, Route("api/[controller]"), Authorize]
public class GanhosController(IGanhoService service) : ControllerBase
{
    private int UsuarioId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> Get()
        => Ok(await service.ObterTodosAsync(UsuarioId));

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] GanhoRequest request)
    {
        var result = await service.CriarAsync(UsuarioId, request);
        return Created($"api/ganhos/{result.Id}", result);
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

