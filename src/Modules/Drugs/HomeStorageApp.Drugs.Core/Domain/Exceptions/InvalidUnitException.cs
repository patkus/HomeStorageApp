namespace HomeStorageApp.Drugs.Core.Domain.Exceptions;

/// <summary>
/// Wyjątek rzucany gdy jednostka miary jest nieprawidłowa lub nie istnieje
/// </summary>
public sealed class InvalidUnitException : DrugDomainException
{
    /// <summary>
    /// Tworzy nową instancję wyjątku dla nieprawidłowej jednostki
    /// </summary>
    /// <param name="unitId">Identyfikator nieprawidłowej jednostki</param>
    public InvalidUnitException(Guid unitId) 
        : base($"Jednostka o ID '{unitId}' nie istnieje lub jest nieprawidłowa")
    {
        UnitId = unitId;
    }

    /// <summary>
    /// Tworzy nową instancję wyjątku z niestandardowym komunikatem
    /// </summary>
    /// <param name="message">Komunikat błędu</param>
    public InvalidUnitException(string message) : base(message)
    {
    }

    /// <summary>
    /// Identyfikator nieprawidłowej jednostki
    /// </summary>
    public Guid? UnitId { get; }
}
