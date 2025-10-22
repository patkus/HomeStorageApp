namespace HomeStorageApp.Identity.Core.Infrastructure.Security;

/// <summary>
/// Konfiguracja JWT (JSON Web Token) dla modułu Identity.
/// Zawiera wszystkie parametry potrzebne do generowania i walidacji tokenów.
/// Wartości są ładowane z appsettings.json w sekcji "Jwt".
/// </summary>
public sealed class JwtSettings
{
    /// <summary>
    /// Sekcja w appsettings.json gdzie znajdują się ustawienia JWT.
    /// </summary>
    public const string SectionName = "Jwt";

    /// <summary>
    /// Tajny klucz używany do podpisywania tokenów JWT.
    /// Musi mieć minimum 256 bitów (32 znaki) dla HS256.
    /// UWAGA: W produkcji przechowuj w bezpiecznym miejscu (Azure Key Vault, AWS Secrets Manager, etc.)
    /// </summary>
    public required string Secret { get; init; }

    /// <summary>
    /// Wydawca tokena (Issuer) - identyfikuje aplikację, która wygenerowała token.
    /// Powinien być unikalny dla Twojej aplikacji (np. domena).
    /// </summary>
    public required string Issuer { get; init; }

    /// <summary>
    /// Odbiorca tokena (Audience) - identyfikuje aplikację, która ma konsumować token.
    /// Zazwyczaj taki sam jak Issuer dla simple API.
    /// </summary>
    public required string Audience { get; init; }

    /// <summary>
    /// Czas wygaśnięcia access tokena w minutach.
    /// Zalecana wartość: 15 minut (bezpieczeństwo vs wygoda).
    /// </summary>
    public required int AccessTokenExpirationMinutes { get; init; }

    /// <summary>
    /// Czas wygaśnięcia refresh tokena w dniach.
    /// Zalecana wartość: 7 dni (balance między bezpieczeństwem a UX).
    /// </summary>
    public required int RefreshTokenExpirationDays { get; init; }
}
