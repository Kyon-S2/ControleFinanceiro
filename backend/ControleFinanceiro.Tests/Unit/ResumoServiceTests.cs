using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ControleFinanceiro.Application.Services;
using ControleFinanceiro.Domain.Entities;
using ControleFinanceiro.Infrastructure.Data;

namespace ControleFinanceiro.Tests.Unit;

public class ResumoServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly AppDbContext _db;
    private readonly ResumoService _service;
    private readonly Guid _usuarioId;

    public ResumoServiceTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _db = new AppDbContext(options);
        _db.Database.EnsureCreated();

        _usuarioId = Guid.NewGuid();
        _db.Usuarios.Add(new Usuario
        {
            Id = _usuarioId,
            Email = "teste@exemplo.com",
            EmailNormalizado = "teste@exemplo.com"
        });
        _db.SaveChanges();

        _service = new ResumoService(_db);
    }

    [Fact]
    public async Task ObterResumo_GastosAbaixoDe80Porcento_RetornaStatusOk()
    {
        _db.LimitesMensais.Add(new LimiteMensal { UsuarioId = _usuarioId, Valor = 1000m });
        var cat = new Categoria { Nome = "Alimentação", UsuarioId = _usuarioId };
        _db.Categorias.Add(cat);
        _db.SaveChanges();

        _db.Gastos.Add(new Gasto { Nome = "Mercado", Valor = 750m, Data = new DateOnly(2026, 11, 10), CategoriaId = cat.Id, UsuarioId = _usuarioId });
        _db.Ganhos.Add(new Ganho { Nome = "Salário", Valor = 3000m, Data = new DateOnly(2026, 11, 5), CategoriaId = cat.Id, UsuarioId = _usuarioId });
        await _db.SaveChangesAsync();

        var resumo = await _service.ObterResumoAsync(_usuarioId, "2026-11");

        Assert.Equal(750m, resumo.TotalGastos);
        Assert.Equal(3000m, resumo.TotalGanhos);
        Assert.Equal(2250m, resumo.Saldo);
        Assert.Equal(1000m, resumo.Limite);
        Assert.Equal(75m, resumo.PercentualLimite);
        Assert.Equal("Ok", resumo.StatusLimite);
        Assert.Equal(1, resumo.QtdGastos);
        Assert.Equal(750m, resumo.MaiorGasto);
    }

    [Fact]
    public async Task ObterResumo_GastosEntre80ECemPorcento_RetornaStatusAlerta80()
    {
        _db.LimitesMensais.Add(new LimiteMensal { UsuarioId = _usuarioId, Valor = 1000m });
        var cat = new Categoria { Nome = "Geral", UsuarioId = _usuarioId };
        _db.Categorias.Add(cat);
        _db.SaveChanges();

        _db.Gastos.Add(new Gasto { Nome = "Compras", Valor = 850m, Data = new DateOnly(2026, 11, 10), CategoriaId = cat.Id, UsuarioId = _usuarioId });
        await _db.SaveChangesAsync();

        var resumo = await _service.ObterResumoAsync(_usuarioId, "2026-11");

        Assert.Equal(850m, resumo.TotalGastos);
        Assert.Equal(85m, resumo.PercentualLimite);
        Assert.Equal("Alerta80", resumo.StatusLimite);
    }

    [Fact]
    public async Task ObterResumo_GastosCemPorcentoOuMais_RetornaStatusEstourado()
    {
        _db.LimitesMensais.Add(new LimiteMensal { UsuarioId = _usuarioId, Valor = 1000m });
        var cat = new Categoria { Nome = "Geral", UsuarioId = _usuarioId };
        _db.Categorias.Add(cat);
        _db.SaveChanges();

        _db.Gastos.Add(new Gasto { Nome = "Viagem", Valor = 1200m, Data = new DateOnly(2026, 11, 10), CategoriaId = cat.Id, UsuarioId = _usuarioId });
        await _db.SaveChangesAsync();

        var resumo = await _service.ObterResumoAsync(_usuarioId, "2026-11");

        Assert.Equal(1200m, resumo.TotalGastos);
        Assert.Equal(120m, resumo.PercentualLimite);
        Assert.Equal("Estourado", resumo.StatusLimite);
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }
}

