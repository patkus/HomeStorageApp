namespace HomeStorageApp.Identity.Core.Application.DTOs;

/// <summary>
/// Request DTO dla logowania użytkownika.
/// Używany przez endpoint POST /api/auth/login.
/// </summary>
/// <param name="Email">Adres email użytkownika używany do logowania</param>
/// <param name="Password">Hasło użytkownika w formie plaintext</param>
public sealed record LoginRequest(
    string Email,
    string Password);
