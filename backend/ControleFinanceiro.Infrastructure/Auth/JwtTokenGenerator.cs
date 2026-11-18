using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ControleFinanceiro.Application.Interfaces;

namespace ControleFinanceiro.Infrastructure.Auth;

public class JwtTokenGenerator(IConfiguration config) : IJwtTokenGenerator
{
    public string GenerateToken(Guid userId, string email)
    {
        var rawKey = config["Jwt:Key"];
        var keyStr = string.IsNullOrWhiteSpace(rawKey)
            ? "DEV_SECRET_KEY_MIN_32_CHARS_LONG_AUTH_TOKEN_TEST"
            : rawKey;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));

        var claims = new[]
        {
            new Claim("sub", userId.ToString()),
            new Claim("email", email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"] ?? "ControleFinanceiro",
            audience: config["Jwt:Audience"] ?? "ControleFinanceiroApp",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
