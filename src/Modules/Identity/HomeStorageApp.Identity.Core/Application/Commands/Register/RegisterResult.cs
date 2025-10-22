namespace HomeStorageApp.Identity.Core.Application.Commands.Register;

/// <summary>
/// Result zwracany przez RegisterCommandHandler po pomyślnej rejestracji.
/// Zawiera dane użytkownika i wygenerowane tokeny.
/// </summary>
/// <param name="UserId">Unikalny identyfikator nowo utworzonego użytkownika</param>
/// <param name="Email">Adres email użytkownika</param>
/// <param name="AccessToken">Wygenerowany JWT access token</param>
/// <param name="RefreshToken">Wygenerowany refresh token</param>
/// <param name="AccessTokenExpiresAt">Data i czas wygaśnięcia access tokena</param>
/// <param name="RefreshTokenExpiresAt">Data i czas wygaśnięcia refresh tokena</param>
public sealed record RegisterResult(
    Guid UserId,
    string Email,
    string AccessToken,
    string RefreshToken,
    DateTimeOffset AccessTokenExpiresAt,
    DateTimeOffset RefreshTokenExpiresAt);
