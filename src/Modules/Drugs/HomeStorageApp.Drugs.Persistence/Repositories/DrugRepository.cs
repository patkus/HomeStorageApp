using HomeStorageApp.Drugs.Core.Application.Interfaces;
using HomeStorageApp.Drugs.Core.Domain.Drugs;
using Microsoft.EntityFrameworkCore;

namespace HomeStorageApp.Drugs.Persistence.Repositories;

/// <summary>
/// Implementacja repozytorium dla encji Drug
/// </summary>
public sealed class DrugRepository(DrugsDbContext context) : IDrugRepository
{
    /// <summary>
    /// Pobiera lek po identyfikatorze dla konkretnego użytkownika
    /// </summary>
    public async Task<Drug?> GetByIdAsync(
        Guid drugId,
        Guid userId,
        bool includeArchived = false,
        CancellationToken cancellationToken = default)
    {
        var query = context.Drugs
            .Include(d => d.PrimaryUnit)
            .Include(d => d.DerivedUnits)
            .Where(d => d.Id == drugId && d.UserId == userId);

        if (!includeArchived)
        {
            query = query.Where(d => !d.IsArchived);
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Pobiera stronicowaną listę leków dla użytkownika
    /// </summary>
    public async Task<(List<Drug> Drugs, int TotalCount)> GetPagedAsync(
        Guid userId,
        int page,
        int pageSize,
        string? sortBy = null,
        string? filterName = null,
        bool includeArchived = false,
        CancellationToken cancellationToken = default)
    {
        var query = context.Drugs
            .Include(d => d.PrimaryUnit)
            .Include(d => d.DerivedUnits)
            .Where(d => d.UserId == userId);

        // Filtrowanie po archiwizacji
        if (!includeArchived)
        {
            query = query.Where(d => !d.IsArchived);
        }

        // Filtrowanie po nazwie
        if (!string.IsNullOrWhiteSpace(filterName))
        {
            query = query.Where(d => d.Name.Contains(filterName));
        }

        // Pobranie całkowitej liczby rekordów przed paginacją
        var totalCount = await query.CountAsync(cancellationToken);

        // Sortowanie
        query = sortBy?.ToLower() switch
        {
            "name" => query.OrderBy(d => d.Name),
            "createdat" => query.OrderBy(d => d.CreatedAt),
            "updatedat" => query.OrderByDescending(d => d.UpdatedAt),
            _ => query.OrderBy(d => d.Name) // Domyślne sortowanie
        };

        // Paginacja
        var drugs = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (drugs, totalCount);
    }

    /// <summary>
    /// Sprawdza czy istnieje lek z danym kodem kreskowym
    /// </summary>
    public async Task<bool> ExistsByBarcodeAsync(
        string barcode,
        Guid? excludeDrugId = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.Drugs
            .Where(d => d.Barcodes.Contains(barcode));

        if (excludeDrugId.HasValue)
        {
            query = query.Where(d => d.Id != excludeDrugId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Dodaje nowy lek do repozytorium
    /// </summary>
    public async Task AddAsync(Drug drug, CancellationToken cancellationToken = default)
    {
        await context.Drugs.AddAsync(drug, cancellationToken);
    }

    /// <summary>
    /// Aktualizuje istniejący lek
    /// </summary>
    public Task UpdateAsync(Drug drug, CancellationToken cancellationToken = default)
    {
        context.Drugs.Update(drug);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Zapisuje zmiany do bazy danych
    /// </summary>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
}
