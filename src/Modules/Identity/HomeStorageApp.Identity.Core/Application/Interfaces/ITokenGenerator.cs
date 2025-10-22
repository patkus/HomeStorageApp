namespace HomeStorageApp.Identity.Core.Application.Interfaces;

/// <summary>
/// Kontrakt dla serwisu generowania tokenów JWT oraz refresh tokenów.
/// Walidacja JWT jest obsługiwana automatycznie przez ASP.NET Core JWT Bearer middleware.
/// </summary>
public interface ITokenGenerator
{
    /// <summary>
    /// Generuje access token JWT dla użytkownika.
    /// Token zawiera claims: Sub (userId), Email, Jti (unique token id), NameIdentifier.
    /// Domyślny czas wygaśnięcia: 15 minut.
    /// </summary>
    /// <param name="userId">Unikalny identyfikator użytkownika</param>
    /// <param name="email">Adres email użytkownika</param>
    /// <returns>Podpisany token JWT jako string</returns>
    string GenerateAccessToken(Guid userId, string email);

    /// <summary>
    /// Generuje refresh token używając kryptograficznie bezpiecznego generatora liczb losowych.
    /// Token to 64 bajty losowych danych zakodowanych w base64.
    /// Domyślny czas wygaśnięcia: 7 dni (zarządzane w bazie danych).
    /// </summary>
    /// <returns>Losowy refresh token jako base64 string</returns>
    string GenerateRefreshToken();
}
