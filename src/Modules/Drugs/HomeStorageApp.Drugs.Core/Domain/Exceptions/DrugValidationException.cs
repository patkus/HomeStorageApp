using HomeStorageApp.Shared.Exceptions;

namespace HomeStorageApp.Drugs.Core.Domain.Exceptions;

/// <summary>
/// Wyjątek rzucany gdy dane nie przechodzą walidacji
/// </summary>
public sealed class DrugValidationException : ValidationException
{
    /// <summary>
    /// Tworzy nową instancję wyjątku walidacji
    /// </summary>
    /// <param name="propertyName">Nazwa właściwości która nie przeszła walidacji</param>
    /// <param name="message">Komunikat błędu</param>
    public DrugValidationException(string propertyName, string message) 
        : base($"{propertyName}: {message}")
    {
    }
}
