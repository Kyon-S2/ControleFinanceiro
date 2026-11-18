using Microsoft.EntityFrameworkCore;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain.Entities;

namespace ControleFinanceiro.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Gasto> Gastos => Set<Gasto>();
    public DbSet<Ganho> Ganhos => Set<Ganho>();
    public DbSet<LimiteMensal> LimitesMensais => Set<LimiteMensal>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // ── Usuario ──
        mb.Entity<Usuario>(e =>
        {
            e.Property(u => u.Email).HasMaxLength(256).IsRequired();
            e.Property(u => u.EmailNormalizado).HasMaxLength(256).IsRequired();
            e.Property(u => u.SenhaHash).HasMaxLength(256);
            e.Property(u => u.GoogleSubject).HasMaxLength(128);

            e.HasIndex(u => u.EmailNormalizado).IsUnique();
            e.HasIndex(u => u.GoogleSubject).IsUnique()
             .HasFilter("\"GoogleSubject\" IS NOT NULL");
        });

        // ── Categoria ──
        mb.Entity<Categoria>(e =>
        {
            e.Property(c => c.Nome).HasMaxLength(100).IsRequired();
            e.HasOne(c => c.Usuario).WithMany(u => u.Categorias)
             .HasForeignKey(c => c.UsuarioId).OnDelete(DeleteBehavior.Cascade);
        });

        // ── Gasto ──
        mb.Entity<Gasto>(e =>
        {
            e.Property(g => g.Nome).HasMaxLength(200).IsRequired();
            e.Property(g => g.Valor).HasPrecision(18, 2);
            e.HasOne(g => g.Categoria).WithMany(c => c.Gastos)
             .HasForeignKey(g => g.CategoriaId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(g => g.Usuario).WithMany(u => u.Gastos)
             .HasForeignKey(g => g.UsuarioId).OnDelete(DeleteBehavior.Cascade);
        });

        // ── Ganho ──
        mb.Entity<Ganho>(e =>
        {
            e.Property(g => g.Nome).HasMaxLength(200).IsRequired();
            e.Property(g => g.Valor).HasPrecision(18, 2);
            e.HasOne(g => g.Categoria).WithMany(c => c.Ganhos)
             .HasForeignKey(g => g.CategoriaId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(g => g.Usuario).WithMany(u => u.Ganhos)
             .HasForeignKey(g => g.UsuarioId).OnDelete(DeleteBehavior.Cascade);
        });

        // ── LimiteMensal ──
        mb.Entity<LimiteMensal>(e =>
        {
            e.Property(l => l.Valor).HasPrecision(18, 2);
            e.HasIndex(l => l.UsuarioId).IsUnique();
            e.HasOne(l => l.Usuario).WithOne(u => u.LimiteMensal)
             .HasForeignKey<LimiteMensal>(l => l.UsuarioId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}

