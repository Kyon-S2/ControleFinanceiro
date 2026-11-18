using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ControleFinanceiro.Application.DTOs;

namespace ControleFinanceiro.Tests.Integration;

public class AuthAndGastosIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthAndGastosIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task FluxoCompleto_RegistroLoginECriarGasto_Sucesso()
    {
        var email = $"novo.usuario+{Guid.NewGuid():N}@gmail.com";
        var senha = "SenhaSegura123";

        // 1. Registro
        var regResp = await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest(email, senha));
        Assert.Equal(HttpStatusCode.Created, regResp.StatusCode);

        // 2. Login
        var loginResp = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest(email, senha));
        Assert.Equal(HttpStatusCode.OK, loginResp.StatusCode);

        var authData = await loginResp.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(authData);
        Assert.False(string.IsNullOrWhiteSpace(authData.Token));

        // 3. Criar Categoria autenticado
        var catRequest = new HttpRequestMessage(HttpMethod.Post, "/api/categorias")
        {
            Content = JsonContent.Create(new CategoriaRequest("Transporte"))
        };
        catRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authData.Token);

        var catResp = await _client.SendAsync(catRequest);
        Assert.Equal(HttpStatusCode.Created, catResp.StatusCode);

        var categoria = await catResp.Content.ReadFromJsonAsync<CategoriaResponse>();
        Assert.NotNull(categoria);

        // 4. Criar Gasto autenticado
        var gastoRequest = new HttpRequestMessage(HttpMethod.Post, "/api/gastos")
        {
            Content = JsonContent.Create(new GastoRequest("Gasolina", 250m, new DateOnly(2026, 11, 18), categoria.Id))
        };
        gastoRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authData.Token);

        var gastoResp = await _client.SendAsync(gastoRequest);
        Assert.Equal(HttpStatusCode.Created, gastoResp.StatusCode);

        var gastoCriado = await gastoResp.Content.ReadFromJsonAsync<GastoResponse>();
        Assert.NotNull(gastoCriado);
        Assert.Equal("Gasolina", gastoCriado.Nome);
        Assert.Equal(250m, gastoCriado.Valor);
    }

    [Fact]
    public async Task Registro_EmailNormalizadoDuplicado_RetornaConflito409()
    {
        var id = Guid.NewGuid().ToString("N");
        var emailOriginal = $"teste.duplicado.{id}+alias@gmail.com";
        var emailVariante = $"testeduplicado{id}@gmail.com";
        var senha = "SenhaSegura123";

        var resp1 = await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest(emailOriginal, senha));
        Assert.Equal(HttpStatusCode.Created, resp1.StatusCode);

        var resp2 = await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest(emailVariante, senha));
        Assert.Equal(HttpStatusCode.Conflict, resp2.StatusCode);
    }
}
