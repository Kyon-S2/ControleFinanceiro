namespace ControleFinanceiro.Application.DTOs;

/// <summary>
/// Dados para cadastro de usuário.
/// </summary>
/// <param name="Email">E-mail do usuário</param>
/// <param name="Senha">Senha do usuário (mínimo 8 caracteres)</param>
public record RegisterRequest(string Email, string Senha);

/// <summary>
/// Dados para login do usuário.
/// </summary>
/// <param name="Email">E-mail do usuário</param>
/// <param name="Senha">Senha do usuário</param>
public record LoginRequest(string Email, string Senha);

/// <summary>
/// Dados para autenticação via Google.
/// </summary>
/// <param name="IdToken">Token JWT retornado pelo Google Identity Services</param>
public record GoogleAuthRequest(string IdToken);

/// <summary>
/// Resposta com token JWT.
/// </summary>
/// <param name="Token">Token de acesso JWT</param>
public record AuthResponse(string Token);
