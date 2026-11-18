using Microsoft.EntityFrameworkCore;
using ControleFinanceiro.Domain.Entities;

namespace ControleFinanceiro.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Gasto> Gastos => Set<Gasto>();
    public DbSet<Ganho> Ganhos => Set<Ganho>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Usuario>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
        });

        mb.Entity<Gasto>(e =>
        {
            e.HasOne(g => g.Usuario).WithMany(u => u.Gastos)
             .HasForeignKey(g => g.UsuarioId).OnDelete(DeleteBehavior.Cascade);
        });

        mb.Entity<Ganho>(e =>
        {
            e.HasOne(g => g.Usuario).WithMany(u => u.Ganhos)
             .HasForeignKey(g => g.UsuarioId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}

