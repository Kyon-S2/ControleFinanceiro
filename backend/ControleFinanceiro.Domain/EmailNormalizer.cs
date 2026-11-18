namespace ControleFinanceiro.Domain;

public static class EmailNormalizer
{
    private static readonly HashSet<string> DisposableDomains = new(StringComparer.OrdinalIgnoreCase)
    {
        "tempmail.com", "10minutemail.com", "guerrillamail.com", "mailinator.com",
        "throwawaymail.com", "yopmail.com", "sharklasers.com", "getairmail.com",
        "dispostable.com", "crazymailing.com", "trashmail.com", "nada.ltd",
        "mohmal.com", "inboxkitten.com", "fakemailgenerator.com", "dropmail.me",
        "generator.email", "temp-mail.org", "burnermail.io", "guerrillamail.net"
    };

    public static string Normalize(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return string.Empty;

        var trimmed = email.Trim().ToLowerInvariant();
        var parts = trimmed.Split('@');
        if (parts.Length != 2)
            return trimmed;

        var local = parts[0];
        var domain = parts[1];

        if (domain == "gmail.com" || domain == "googlemail.com")
        {
            var plusIdx = local.IndexOf('+');
            if (plusIdx >= 0)
                local = local[..plusIdx];

            local = local.Replace(".", "");
            domain = "gmail.com";
        }

        return $"{local}@{domain}";
    }

    public static bool IsDisposable(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var parts = email.Trim().Split('@');
        if (parts.Length != 2)
            return false;

        return DisposableDomains.Contains(parts[1].ToLowerInvariant());
    }
}

