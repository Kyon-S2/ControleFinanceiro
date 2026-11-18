namespace ControleFinanceiro.Application.Services;

using ControleFinanceiro.Application.DTOs;

public interface IGanhoService
{
    Task<List<GanhoResponse>> ObterTodosAsync(int usuarioId);
    Task<GanhoResponse> CriarAsync(int usuarioId, GanhoRequest request);
    Task RemoverAsync(int usuarioId, int id);
}

