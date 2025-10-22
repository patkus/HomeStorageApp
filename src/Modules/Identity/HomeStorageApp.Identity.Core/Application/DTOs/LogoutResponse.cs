namespace HomeStorageApp.Identity.Core.Application.DTOs;

/// <summary>
/// Response DTO dla operacji wylogowania.
/// Zwraca informację o powodzeniu operacji.
/// Używany jako odpowiedź dla POST /api/auth/logout.
/// </summary>
/// <param name="Success">Określa, czy operacja wylogowania się powiodła</param>
/// <param name="Message">Komunikat informacyjny dla użytkownika</param>
public sealed record LogoutResponse(
    bool Success,
    string Message);
