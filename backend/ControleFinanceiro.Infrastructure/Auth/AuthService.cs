using Microsoft.EntityFrameworkCore;
using ControleFinanceiro.Application.DTOs;
using ControleFinanceiro.Application.Exceptions;
using ControleFinanceiro.Application.Interfaces;
using ControleFinanceiro.Domain;
using ControleFinanceiro.Domain.Entities;
using ControleFinanceiro.Infrastructure.Data;

namespace ControleFinanceiro.Infrastructure.Auth;

public class AuthService(
    AppDbContext db,
    IJwtTokenGenerator jwtTokenGenerator) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains('@'))
        {
            throw new ValidationException("E-mail inválido.");
        }

        if (EmailNormalizer.IsDisposable(request.Email))
        {
            throw new ValidationException("Domínio de e-mail não permitido.");
        }

        if (string.IsNullOrWhiteSpace(request.Senha) || request.Senha.Length < 8)
        {
            throw new ValidationException("A senha deve ter no mínimo 8 caracteres.");
        }

        var normalizado = EmailNormalizer.Normalize(request.Email);
        var jaExiste = await db.Usuarios.AnyAsync(u => u.EmailNormalizado == normalizado);
        if (jaExiste)
        {
            throw new EmailAlreadyRegisteredException();
        }

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Email = request.Email.Trim(),
            EmailNormalizado = normalizado,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha),
            CriadoEm = DateTime.UtcNow
        };

        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();

        var token = jwtTokenGenerator.GenerateToken(usuario.Id, usuario.Email);
        return new AuthResponse(token);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Senha))
        {
            throw new InvalidCredentialsException();
        }

        var normalizado = EmailNormalizer.Normalize(request.Email);
        var usuario = await db.Usuarios.FirstOrDefaultAsync(u => u.EmailNormalizado == normalizado);

        if (usuario == null || string.IsNullOrEmpty(usuario.SenhaHash))
        {
            throw new InvalidCredentialsException();
        }

        bool senhaValida = false;
        try
        {
            senhaValida = BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash);
        }
        catch
        {
            senhaValida = false;
        }

        if (!senhaValida)
        {
            throw new InvalidCredentialsException();
        }

        var token = jwtTokenGenerator.GenerateToken(usuario.Id, usuario.Email);
        return new AuthResponse(token);
    }
}
