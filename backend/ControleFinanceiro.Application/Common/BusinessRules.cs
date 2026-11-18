using ControleFinanceiro.Application.Exceptions;

namespace ControleFinanceiro.Application.Common;

public static class BusinessRules
{
    public static void ValidateNome(string? nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ValidationException("O nome é obrigatório.");

        var trimmed = nome.Trim();
        if (trimmed.All(char.IsDigit))
            throw new ValidationException("O nome não pode conter apenas dígitos.");
    }

    public static void ValidateValor(decimal valor)
    {
        if (valor <= 0)
            throw new ValidationException("O valor deve ser maior que zero.");
    }
}

