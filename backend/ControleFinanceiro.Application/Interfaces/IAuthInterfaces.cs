namespace ControleFinanceiro.Application.Interfaces;

using ControleFinanceiro.Application.DTOs;

public record GoogleUserInfo(string Subject, string Email, bool EmailVerified);

public interface IGoogleAuthValidator
{
    Task<GoogleUserInfo> ValidateAsync(string idToken);
}

public interface IJwtTokenGenerator
{
    string GenerateToken(Guid userId, string email);
}

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> GoogleAsync(GoogleAuthRequest request);
}

