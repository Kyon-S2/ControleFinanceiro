using System.ComponentModel.DataAnnotations;

namespace ControleFinanceiro.Application.DTOs;

public record LoginRequest(
    [Required] string Email,
    [Required] string Senha);

public record RegistroRequest(
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Senha);

public record AuthResponse(string Token, string Email);

public record GastoRequest(
    [Required, MaxLength(200)] string Nome,
    [Range(0.01, double.MaxValue)] decimal Valor,
    [Required, MaxLength(100)] string Classe,
    [Required, MaxLength(10)] string Data);

public record GastoResponse(int Id, string Nome, decimal Valor, string Classe, string Data);

public record GanhoRequest(
    [Required, MaxLength(200)] string Nome,
    [Range(0.01, double.MaxValue)] decimal Valor,
    [Required, MaxLength(100)] string Classe,
    [Required, MaxLength(10)] string Data,
    bool Fixo);

public record GanhoResponse(int Id, string Nome, decimal Valor, string Classe, string Data, bool Fixo);

