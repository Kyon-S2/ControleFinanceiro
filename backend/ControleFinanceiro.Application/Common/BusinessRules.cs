using ControleFinanceiro.Application.Exceptions;

namespace ControleFinanceiro.Application.Common;

public static class BusinessRules
{
    public const int MaxLengthCategoriaNome = 100;
    public const int MaxLengthLancamentoNome = 200;

    public static void ValidateNome(string? nome, int maxLength = MaxLengthLancamentoNome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ValidacaoException("O nome é obrigatório.");

        var trimmed = nome.Trim();
        if (trimmed.All(char.IsDigit))
            throw new ValidacaoException("O nome não pode conter apenas dígitos.");

        if (trimmed.Length > maxLength)
            throw new ValidacaoException($"O nome deve ter no máximo {maxLength} caracteres.");
    }

    public static void ValidateCategoriaNome(string? nome)
    {
        ValidateNome(nome, MaxLengthCategoriaNome);
    }

    public static void ValidateValor(decimal valor)
    {
        if (valor <= 0)
            throw new ValidacaoException("O valor deve ser maior que zero.");
    }
}
