namespace HomeStorageApp.Identity.Core.Application.DTOs;

/// <summary>
/// Request DTO dla rejestracji nowego użytkownika.
/// Używany przez endpoint POST /api/auth/register.
/// </summary>
/// <param name="Email">Adres email użytkownika (unikalny w systemie)</param>
/// <param name="Password">Hasło użytkownika (min 8 znaków, wymaga: 1 wielka, 1 mała, 1 cyfra, 1 znak specjalny)</param>
/// <param name="ConfirmPassword">Potwierdzenie hasła (musi być identyczne z Password)</param>
public sealed record RegisterRequest(
    string Email,
    string Password,
    string ConfirmPassword);
