namespace HomeStorageApp.Identity.Core.Application.Commands.Login;

/// <summary>
/// Result zwracany przez LoginCommandHandler po pomyślnym logowaniu.
/// Zawiera dane użytkownika i wygenerowane tokeny.
/// </summary>
/// <param name="UserId">Unikalny identyfikator użytkownika</param>
/// <param name="Email">Adres email użytkownika</param>
/// <param name="AccessToken">Wygenerowany JWT access token</param>
/// <param name="RefreshToken">Wygenerowany refresh token</param>
/// <param name="AccessTokenExpiresAt">Data i czas wygaśnięcia access tokena</param>
/// <param name="RefreshTokenExpiresAt">Data i czas wygaśnięcia refresh tokena</param>
public sealed record LoginResult(
    Guid UserId,
    string Email,
    string AccessToken,
    string RefreshToken,
    DateTimeOffset AccessTokenExpiresAt,
    DateTimeOffset RefreshTokenExpiresAt);
