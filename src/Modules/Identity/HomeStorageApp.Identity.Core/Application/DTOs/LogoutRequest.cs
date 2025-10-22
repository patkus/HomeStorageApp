namespace HomeStorageApp.Identity.Core.Application.DTOs;

/// <summary>
/// Request DTO dla wylogowania użytkownika.
/// Używany przez endpoint POST /api/auth/logout.
/// Wymaga autoryzacji (Bearer token w header).
/// </summary>
/// <param name="RefreshToken">Refresh token do unieważnienia (base64 string)</param>
public sealed record LogoutRequest(string RefreshToken);
