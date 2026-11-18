using Microsoft.EntityFrameworkCore;
using ControleFinanceiro.Domain.Entities;

namespace ControleFinanceiro.Application.Interfaces;

public interface IAppDbContext
{
    DbSet<Usuario> Usuarios { get; }
    DbSet<Categoria> Categorias { get; }
    DbSet<Gasto> Gastos { get; }
    DbSet<Ganho> Ganhos { get; }
    DbSet<LimiteMensal> LimitesMensais { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

