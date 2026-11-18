using System.ComponentModel.DataAnnotations;

namespace ControleFinanceiro.Domain.Entities;

public class Gasto
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Nome { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Valor { get; set; }

    [Required, MaxLength(100)]
    public string Classe { get; set; } = string.Empty;

    [Required, MaxLength(10)]
    public string Data { get; set; } = string.Empty;

    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
}

