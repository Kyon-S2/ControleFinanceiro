using ControleFinanceiro.Application.DTOs;
using ControleFinanceiro.Application.Services;
using ControleFinanceiro.Domain.Entities;
using ControleFinanceiro.Domain.Interfaces;

namespace ControleFinanceiro.Infrastructure.Auth;

public class AuthService(IUsuarioRepository repo, JwtService jwt) : IAuthService
{
    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var usuario = await repo.ObterPorEmailAsync(request.Email.ToLowerInvariant())
            ?? throw new UnauthorizedAccessException("E-mail ou senha incorretos.");

        if (!BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash))
            throw new UnauthorizedAccessException("E-mail ou senha incorretos.");

        var token = jwt.GerarToken(usuario.Id, usuario.Email);
        return new AuthResponse(token, usuario.Email);
    }

    public async Task<AuthResponse> RegistrarAsync(RegistroRequest request)
    {
        var emailNorm = request.Email.ToLowerInvariant();
        var existente = await repo.ObterPorEmailAsync(emailNorm);
        if (existente is not null)
            throw new InvalidOperationException("E-mail já cadastrado.");

        var usuario = new Usuario
        {
            Email = emailNorm,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha)
        };

        await repo.CriarAsync(usuario);
        var token = jwt.GerarToken(usuario.Id, usuario.Email);
        return new AuthResponse(token, usuario.Email);
    }
}

