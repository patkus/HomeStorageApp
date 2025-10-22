namespace HomeStorageApp.Shared.Exceptions;

/// <summary>
/// Bazowy wyjątek dla przypadków gdy zasób nie został znaleziony.
/// Mapowany na HTTP 404 Not Found.
/// </summary>
/// <param name="message">Komunikat opisujący brak zasobu</param>
public abstract class NotFoundException(string message) : DomainException(message);
