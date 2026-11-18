namespace ControleFinanceiro.Domain.Interfaces;

using ControleFinanceiro.Domain.Entities;

public interface IGanhoRepository
{
    Task<List<Ganho>> ObterTodosPorUsuarioAsync(int usuarioId);
    Task<Ganho?> ObterPorIdAsync(int id, int usuarioId);
    Task<Ganho> CriarAsync(Ganho ganho);
    Task RemoverAsync(Ganho ganho);
}

