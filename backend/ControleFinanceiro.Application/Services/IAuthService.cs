namespace ControleFinanceiro.Application.Services;

using ControleFinanceiro.Application.DTOs;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegistrarAsync(RegistroRequest request);
}

