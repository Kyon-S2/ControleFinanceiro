namespace ControleFinanceiro.Domain.Entities;

public class Usuario
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string EmailNormalizado { get; set; } = string.Empty;
    public string? SenhaHash { get; set; }
    public string? GoogleSubject { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public ICollection<Categoria> Categorias { get; set; } = [];
    public ICollection<Gasto> Gastos { get; set; } = [];
    public ICollection<Ganho> Ganhos { get; set; } = [];
    public LimiteMensal? LimiteMensal { get; set; }
}

