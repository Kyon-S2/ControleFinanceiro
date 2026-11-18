namespace ControleFinanceiro.Domain.Entities;

public class Gasto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateOnly Data { get; set; }
    public int CategoriaId { get; set; }
    public Categoria Categoria { get; set; } = null!;
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}

