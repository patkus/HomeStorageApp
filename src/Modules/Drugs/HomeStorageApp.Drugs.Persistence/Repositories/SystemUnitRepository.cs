using HomeStorageApp.Drugs.Core.Application.Interfaces;
using HomeStorageApp.Drugs.Core.Domain.SystemUnits;
using Microsoft.EntityFrameworkCore;

namespace HomeStorageApp.Drugs.Persistence.Repositories;

/// <summary>
/// Implementacja repozytorium dla encji SystemUnit
/// </summary>
public sealed class SystemUnitRepository(DrugsDbContext context) : ISystemUnitRepository
{
    /// <summary>
    /// Pobiera jednostkę systemową po identyfikatorze
    /// </summary>
    public async Task<SystemUnit?> GetByIdAsync(Guid unitId, CancellationToken cancellationToken = default)
    {
        return await context.SystemUnits
            .FirstOrDefaultAsync(su => su.Id == unitId, cancellationToken);
    }

    /// <summary>
    /// Pobiera wszystkie jednostki systemowe
    /// </summary>
    public async Task<List<SystemUnit>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.SystemUnits
            .OrderBy(su => su.Category)
            .ThenBy(su => su.Name)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Pobiera jednostki systemowe według kategorii
    /// </summary>
    public async Task<List<SystemUnit>> GetByCategoryAsync(
        SystemUnitCategory category,
        CancellationToken cancellationToken = default)
    {
        return await context.SystemUnits
            .Where(su => su.Category == category)
            .OrderBy(su => su.Name)
            .ToListAsync(cancellationToken);
    }
}
