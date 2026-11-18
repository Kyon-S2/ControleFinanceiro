using System.Security.Claims;

namespace ControleFinanceiro.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUsuarioId(this ClaimsPrincipal principal)
    {
        var claimValue = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? principal.FindFirst("sub")?.Value;

        if (Guid.TryParse(claimValue, out var guid))
            return guid;

        throw new UnauthorizedAccessException("Usuário não identificado.");
    }
}

