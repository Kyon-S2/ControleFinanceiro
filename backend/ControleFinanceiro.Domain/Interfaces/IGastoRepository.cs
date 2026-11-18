namespace ControleFinanceiro.Domain.Interfaces;

using ControleFinanceiro.Domain.Entities;

public interface IGastoRepository
{
    Task<List<Gasto>> ObterTodosPorUsuarioAsync(int usuarioId);
    Task<Gasto?> ObterPorIdAsync(int id, int usuarioId);
    Task<Gasto> CriarAsync(Gasto gasto);
    Task<Gasto> AtualizarAsync(Gasto gasto);
    Task RemoverAsync(Gasto gasto);
}

