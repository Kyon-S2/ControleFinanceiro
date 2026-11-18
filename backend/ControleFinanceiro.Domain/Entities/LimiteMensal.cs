namespace ControleFinanceiro.Domain.Entities;

public class LimiteMensal
{
    public int Id { get; set; }
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public decimal Valor { get; set; }
}

