using HomeStorageApp.Shared.Exceptions;

namespace HomeStorageApp.Identity.Core.Domain.Errors;

/// <summary>
/// Wyjątek rzucany, gdy użytkownik próbuje się zalogować, ale jego konto jest nieaktywne.
/// Zwracany jako odpowiedź 403 Forbidden w API.
/// </summary>
public sealed class UserNotActiveException() 
    : ForbiddenException("Konto użytkownika jest nieaktywne");
