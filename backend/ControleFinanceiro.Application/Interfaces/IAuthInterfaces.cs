namespace ControleFinanceiro.Application.Interfaces;

using ControleFinanceiro.Application.DTOs;

public interface IJwtTokenGenerator
{
    string GenerateToken(Guid userId, string email);
}

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}

