using HomeStorageApp.Shared.Exceptions;

namespace HomeStorageApp.Identity.Core.Domain.Errors;

/// <summary>
/// Wyjątek rzucany, gdy użytkownik poda nieprawidłowe dane logowania (email lub hasło).
/// Zwracany jako odpowiedź 401 Unauthorized w API.
/// Z bezpieczeństwa nie ujawniamy, czy problem dotyczy email czy hasła.
/// </summary>
public sealed class InvalidCredentialsException() 
    : UnauthorizedException("Email lub hasło jest nieprawidłowe");
