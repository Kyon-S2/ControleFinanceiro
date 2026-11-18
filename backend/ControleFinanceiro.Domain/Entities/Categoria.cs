namespace ControleFinanceiro.Domain.Entities;

public class Categoria
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    public ICollection<Gasto> Gastos { get; set; } = [];
    public ICollection<Ganho> Ganhos { get; set; } = [];
}

