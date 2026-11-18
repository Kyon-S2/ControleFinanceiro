using Microsoft.EntityFrameworkCore;
using ControleFinanceiro.Domain.Entities;
using ControleFinanceiro.Domain.Interfaces;
using ControleFinanceiro.Infrastructure.Data;

namespace ControleFinanceiro.Infrastructure.Repositories;

public class GastoRepository(AppDbContext db) : IGastoRepository
{
    public Task<List<Gasto>> ObterTodosPorUsuarioAsync(int usuarioId)
        => db.Gastos.Where(g => g.UsuarioId == usuarioId).OrderBy(g => g.Id).ToListAsync();

    public Task<Gasto?> ObterPorIdAsync(int id, int usuarioId)
        => db.Gastos.FirstOrDefaultAsync(g => g.Id == id && g.UsuarioId == usuarioId);

    public async Task<Gasto> CriarAsync(Gasto gasto)
    {
        db.Gastos.Add(gasto);
        await db.SaveChangesAsync();
        return gasto;
    }

    public async Task<Gasto> AtualizarAsync(Gasto gasto)
    {
        db.Gastos.Update(gasto);
        await db.SaveChangesAsync();
        return gasto;
    }

    public async Task RemoverAsync(Gasto gasto)
    {
        db.Gastos.Remove(gasto);
        await db.SaveChangesAsync();
    }
}

