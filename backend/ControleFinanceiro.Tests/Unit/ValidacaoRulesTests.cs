using ControleFinanceiro.Application.Common;
using ControleFinanceiro.Application.Exceptions;

namespace ControleFinanceiro.Tests.Unit;

public class ValidacaoRulesTests
{
    [Theory]
    [InlineData("12345")]
    [InlineData("000")]
    public void ValidateNome_NomeApenasComDigitos_LancaExcecao(string nomeInvalido)
    {
        Assert.Throws<ValidacaoException>(() => BusinessRules.ValidateNome(nomeInvalido));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ValidateNome_NomeVazioOuNulo_LancaExcecao(string? nomeVazio)
    {
        Assert.Throws<ValidacaoException>(() => BusinessRules.ValidateNome(nomeVazio));
    }

    [Theory]
    [InlineData("Aluguel")]
    [InlineData("Salário 11/2026")]
    [InlineData("Gasto 123")]
    public void ValidateNome_NomeValido_NaoLancaExcecao(string nomeValido)
    {
        var exception = Record.Exception(() => BusinessRules.ValidateNome(nomeValido));
        Assert.Null(exception);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    [InlineData(-100)]
    public void ValidateValor_ValorMenorOuIgualAZero_LancaExcecao(decimal valorInvalido)
    {
        Assert.Throws<ValidacaoException>(() => BusinessRules.ValidateValor(valorInvalido));
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(100.50)]
    [InlineData(5000)]
    public void ValidateValor_ValorPositivo_NaoLancaExcecao(decimal valorValido)
    {
        var exception = Record.Exception(() => BusinessRules.ValidateValor(valorValido));
        Assert.Null(exception);
    }
}

