namespace HomeStorageApp.Identity.Core.Application.DTOs;

/// <summary>
/// Response DTO dla operacji rejestracji i logowania.
/// Zwraca komplet tokenów i informacji o użytkowniku.
/// Używany jako odpowiedź dla POST /api/auth/register i POST /api/auth/login.
/// </summary>
/// <param name="UserId">Unikalny identyfikator użytkownika</param>
/// <param name="Email">Adres email użytkownika</param>
/// <param name="AccessToken">JWT access token (ważny 15 minut)</param>
/// <param name="RefreshToken">Refresh token (ważny 7 dni)</param>
/// <param name="AccessTokenExpiresAt">Data i czas wygaśnięcia access tokena (UTC)</param>
/// <param name="RefreshTokenExpiresAt">Data i czas wygaśnięcia refresh tokena (UTC)</param>
public sealed record AuthResponse(
    Guid UserId,
    string Email,
    string AccessToken,
    string RefreshToken,
    DateTimeOffset AccessTokenExpiresAt,
    DateTimeOffset RefreshTokenExpiresAt);
