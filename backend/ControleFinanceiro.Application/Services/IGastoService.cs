namespace ControleFinanceiro.Application.Services;

using ControleFinanceiro.Application.DTOs;

public interface IGastoService
{
    Task<List<GastoResponse>> ObterTodosAsync(int usuarioId);
    Task<GastoResponse> CriarAsync(int usuarioId, GastoRequest request);
    Task<GastoResponse> AtualizarAsync(int usuarioId, int id, GastoRequest request);
    Task RemoverAsync(int usuarioId, int id);
}

