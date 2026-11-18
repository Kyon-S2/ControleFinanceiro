using Microsoft.EntityFrameworkCore;
using ControleFinanceiro.Domain.Entities;
using ControleFinanceiro.Domain.Interfaces;
using ControleFinanceiro.Infrastructure.Data;

namespace ControleFinanceiro.Infrastructure.Repositories;

public class GanhoRepository(AppDbContext db) : IGanhoRepository
{
    public Task<List<Ganho>> ObterTodosPorUsuarioAsync(int usuarioId)
        => db.Ganhos.Where(g => g.UsuarioId == usuarioId).OrderBy(g => g.Id).ToListAsync();

    public Task<Ganho?> ObterPorIdAsync(int id, int usuarioId)
        => db.Ganhos.FirstOrDefaultAsync(g => g.Id == id && g.UsuarioId == usuarioId);

    public async Task<Ganho> CriarAsync(Ganho ganho)
    {
        db.Ganhos.Add(ganho);
        await db.SaveChangesAsync();
        return ganho;
    }

    public async Task RemoverAsync(Ganho ganho)
    {
        db.Ganhos.Remove(ganho);
        await db.SaveChangesAsync();
    }
}

