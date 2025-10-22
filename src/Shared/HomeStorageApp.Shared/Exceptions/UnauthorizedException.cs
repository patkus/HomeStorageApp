namespace HomeStorageApp.Shared.Exceptions;

/// <summary>
/// Bazowy wyjątek dla błędów uwierzytelniania - nieprawidłowe credentials lub tokeny.
/// Mapowany na HTTP 401 Unauthorized.
/// </summary>
/// <param name="message">Komunikat opisujący błąd uwierzytelniania</param>
public abstract class UnauthorizedException(string message) : DomainException(message);
