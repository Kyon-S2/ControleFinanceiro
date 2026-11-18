using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using ControleFinanceiro.Application.Interfaces;

namespace ControleFinanceiro.Infrastructure.Auth;

public class GoogleAuthValidator(IConfiguration config) : IGoogleAuthValidator
{
    public async Task<GoogleUserInfo> ValidateAsync(string idToken)
    {
        var clientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID") ?? config["Google:ClientId"];

        var settings = new GoogleJsonWebSignature.ValidationSettings();
        if (!string.IsNullOrWhiteSpace(clientId))
        {
            settings.Audience = new[] { clientId };
        }

        var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
        return new GoogleUserInfo(payload.Subject, payload.Email, payload.EmailVerified);
    }
}

