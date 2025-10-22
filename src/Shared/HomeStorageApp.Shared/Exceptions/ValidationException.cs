namespace HomeStorageApp.Shared.Exceptions;

/// <summary>
/// Bazowy wyjątek dla błędów walidacji danych wejściowych.
/// Mapowany na HTTP 400 Bad Request.
/// </summary>
/// <param name="message">Komunikat opisujący błąd walidacji</param>
public abstract class ValidationException(string message) : DomainException(message);
