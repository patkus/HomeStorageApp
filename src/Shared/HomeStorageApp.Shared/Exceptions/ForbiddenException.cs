namespace HomeStorageApp.Shared.Exceptions;

/// <summary>
/// Bazowy wyjątek dla błędów autoryzacji - użytkownik nie ma uprawnień do wykonania operacji.
/// Mapowany na HTTP 403 Forbidden.
/// </summary>
/// <param name="message">Komunikat opisujący brak uprawnień</param>
public abstract class ForbiddenException(string message) : DomainException(message);
