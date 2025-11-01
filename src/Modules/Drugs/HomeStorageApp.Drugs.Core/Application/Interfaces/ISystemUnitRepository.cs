using HomeStorageApp.Drugs.Core.Domain.SystemUnits;

namespace HomeStorageApp.Drugs.Core.Application.Interfaces;

/// <summary>
/// Interfejs repozytorium dla encji SystemUnit
/// </summary>
public interface ISystemUnitRepository
{
    /// <summary>
    /// Pobiera jednostkę systemową po identyfikatorze
    /// </summary>
    /// <param name="unitId">Identyfikator jednostki</param>
    /// <param name="cancellationToken">Token anulowania operacji</param>
    /// <returns>Jednostka systemowa lub null jeśli nie znaleziono</returns>
    Task<SystemUnit?> GetByIdAsync(Guid unitId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera wszystkie jednostki systemowe
    /// </summary>
    /// <param name="cancellationToken">Token anulowania operacji</param>
    /// <returns>Lista wszystkich jednostek systemowych</returns>
    Task<List<SystemUnit>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera jednostki systemowe według kategorii
    /// </summary>
    /// <param name="category">Kategoria jednostek</param>
    /// <param name="cancellationToken">Token anulowania operacji</param>
    /// <returns>Lista jednostek z danej kategorii</returns>
    Task<List<SystemUnit>> GetByCategoryAsync(SystemUnitCategory category, CancellationToken cancellationToken = default);
}
