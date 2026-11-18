namespace ControleFinanceiro.Application.DTOs;

// Categoria
public record CategoriaRequest(string Nome);
public record CategoriaResponse(int Id, string Nome);

// Gasto
public record GastoRequest(string Nome, decimal Valor, DateOnly Data, int CategoriaId);
public record GastoResponse(int Id, string Nome, decimal Valor, DateOnly Data, int CategoriaId, string? CategoriaNome, DateTime CriadoEm);

// Ganho
public record GanhoRequest(string Nome, decimal Valor, DateOnly Data, int CategoriaId, bool Fixo);
public record GanhoResponse(int Id, string Nome, decimal Valor, DateOnly Data, int CategoriaId, string? CategoriaNome, bool Fixo, DateTime CriadoEm);

// Limite
public record LimiteRequest(decimal Valor);
public record LimiteResponse(decimal Valor);

// Resumo
public record ResumoResponse(
    decimal TotalGastos,
    int QtdGastos,
    decimal MaiorGasto,
    decimal TotalGanhos,
    decimal Saldo,
    decimal Limite,
    decimal PercentualLimite,
    string StatusLimite
);

