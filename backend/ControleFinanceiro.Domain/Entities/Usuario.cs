using System.ComponentModel.DataAnnotations;

namespace ControleFinanceiro.Domain.Entities;

public class Usuario
{
    public int Id { get; set; }

    [Required, MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string SenhaHash { get; set; } = string.Empty;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public ICollection<Gasto> Gastos { get; set; } = [];
    public ICollection<Ganho> Ganhos { get; set; } = [];
}

