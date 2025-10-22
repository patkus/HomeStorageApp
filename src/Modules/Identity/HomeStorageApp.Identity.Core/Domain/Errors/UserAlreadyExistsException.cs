using HomeStorageApp.Shared.Exceptions;

namespace HomeStorageApp.Identity.Core.Domain.Errors;

/// <summary>
/// Wyjątek rzucany, gdy próbujemy zarejestrować użytkownika z adresem email, który już istnieje w systemie.
/// Zwracany jako odpowiedź 409 Conflict w API.
/// </summary>
/// <param name="email">Adres email, który spowodował konflikt</param>
public sealed class UserAlreadyExistsException(string email) 
    : ConflictException($"Użytkownik z adresem email '{email}' już istnieje");
