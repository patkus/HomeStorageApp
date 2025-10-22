namespace HomeStorageApp.Shared.Exceptions;

/// <summary>
/// Bazowy wyjątek dla konfliktów - gdy zasób już istnieje lub operacja powoduje konflikt stanu.
/// Mapowany na HTTP 409 Conflict.
/// </summary>
/// <param name="message">Komunikat opisujący konflikt</param>
public abstract class ConflictException(string message) : DomainException(message);
