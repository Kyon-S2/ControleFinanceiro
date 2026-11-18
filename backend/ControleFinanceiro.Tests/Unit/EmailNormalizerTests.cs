using ControleFinanceiro.Domain;

namespace ControleFinanceiro.Tests.Unit;

public class EmailNormalizerTests
{
    [Fact]
    public void Normalize_GmailComPontosETag_RemovePontosETag()
    {
        var normalizado = EmailNormalizer.Normalize("usuario.teste+contas@gmail.com");
        Assert.Equal("usuarioteste@gmail.com", normalizado);
    }

    [Fact]
    public void Normalize_Googlemail_NormalizaParaGmail()
    {
        var normalizado = EmailNormalizer.Normalize("u.s.e.r+financas@googlemail.com");
        Assert.Equal("user@gmail.com", normalizado);
    }

    [Fact]
    public void Normalize_OutrosDominios_MantemPontosEMinusculas()
    {
        var normalizado = EmailNormalizer.Normalize("Nome.Sobrenome+tag@Outlook.COM");
        Assert.Equal("nome.sobrenome+tag@outlook.com", normalizado);
    }

    [Theory]
    [InlineData("teste@tempmail.com", true)]
    [InlineData("teste@10minutemail.com", true)]
    [InlineData("teste@mailinator.com", true)]
    [InlineData("teste@gmail.com", false)]
    [InlineData("teste@empresa.com.br", false)]
    public void IsDisposable_IdentificaDominiosDescartaveis(string email, bool esperado)
    {
        var resultado = EmailNormalizer.IsDisposable(email);
        Assert.Equal(esperado, resultado);
    }
}

