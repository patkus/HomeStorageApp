namespace HomeStorageApp.Drugs.Core.Domain.Exceptions;

/// <summary>
/// Wyjątek rzucany gdy wykryto zapętlenie w hierarchii jednostek pochodnych
/// </summary>
public sealed class CircularDependencyException : DrugDomainException
{
    /// <summary>
    /// Tworzy nową instancję wyjątku dla zapętlenia w hierarchii jednostek
    /// </summary>
    /// <param name="unitId">Identyfikator jednostki powodującej zapętlenie</param>
    /// <param name="baseUnitId">Identyfikator jednostki bazowej powodującej zapętlenie</param>
    public CircularDependencyException(Guid unitId, Guid baseUnitId) 
        : base($"Wykryto zapętlenie w hierarchii jednostek: jednostka '{unitId}' nie może mieć jako bazowej jednostki '{baseUnitId}'")
    {
        UnitId = unitId;
        BaseUnitId = baseUnitId;
    }

    /// <summary>
    /// Tworzy nową instancję wyjątku z niestandardowym komunikatem
    /// </summary>
    /// <param name="message">Komunikat błędu</param>
    public CircularDependencyException(string message) : base(message)
    {
    }

    /// <summary>
    /// Identyfikator jednostki powodującej zapętlenie
    /// </summary>
    public Guid? UnitId { get; }

    /// <summary>
    /// Identyfikator jednostki bazowej powodującej zapętlenie
    /// </summary>
    public Guid? BaseUnitId { get; }
}
