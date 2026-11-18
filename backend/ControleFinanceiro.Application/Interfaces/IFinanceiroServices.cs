using ControleFinanceiro.Application.DTOs;

namespace ControleFinanceiro.Application.Interfaces;

public interface ICategoriaService
{
    Task<List<CategoriaResponse>> ObterTodasAsync(Guid usuarioId);
    Task<CategoriaResponse> ObterPorIdAsync(Guid usuarioId, int id);
    Task<CategoriaResponse> CriarAsync(Guid usuarioId, CategoriaRequest request);
    Task<CategoriaResponse> AtualizarAsync(Guid usuarioId, int id, CategoriaRequest request);
    Task RemoverAsync(Guid usuarioId, int id);
}

public interface IGastoService
{
    Task<List<GastoResponse>> ObterTodosAsync(Guid usuarioId, string? nome = null, int? categoriaId = null, string? mes = null);
    Task<GastoResponse> ObterPorIdAsync(Guid usuarioId, int id);
    Task<GastoResponse> CriarAsync(Guid usuarioId, GastoRequest request);
    Task<GastoResponse> AtualizarAsync(Guid usuarioId, int id, GastoRequest request);
    Task RemoverAsync(Guid usuarioId, int id);
}

public interface IGanhoService
{
    Task<List<GanhoResponse>> ObterTodosAsync(Guid usuarioId, string? nome = null, int? categoriaId = null, string? mes = null);
    Task<GanhoResponse> ObterPorIdAsync(Guid usuarioId, int id);
    Task<GanhoResponse> CriarAsync(Guid usuarioId, GanhoRequest request);
    Task<GanhoResponse> AtualizarAsync(Guid usuarioId, int id, GanhoRequest request);
    Task RemoverAsync(Guid usuarioId, int id);
}

public interface ILimiteService
{
    Task<LimiteResponse> ObterAsync(Guid usuarioId);
    Task<LimiteResponse> DefinirAsync(Guid usuarioId, LimiteRequest request);
}

public interface IResumoService
{
    Task<ResumoResponse> ObterResumoAsync(Guid usuarioId, string? mes = null);
}

