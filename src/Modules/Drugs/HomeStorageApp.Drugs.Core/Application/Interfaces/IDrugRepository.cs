using HomeStorageApp.Drugs.Core.Domain.Drugs;

namespace HomeStorageApp.Drugs.Core.Application.Interfaces;

/// <summary>
/// Interfejs repozytorium dla encji Drug
/// </summary>
public interface IDrugRepository
{
    /// <summary>
    /// Pobiera lek po identyfikatorze dla konkretnego użytkownika
    /// </summary>
    /// <param name="drugId">Identyfikator leku</param>
    /// <param name="userId">Identyfikator właściciela</param>
    /// <param name="includeArchived">Czy uwzględnić zarchiwizowane leki</param>
    /// <param name="cancellationToken">Token anulowania operacji</param>
    /// <returns>Lek lub null jeśli nie znaleziono</returns>
    Task<Drug?> GetByIdAsync(Guid drugId, Guid userId, bool includeArchived = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera stronicowaną listę leków dla użytkownika
    /// </summary>
    /// <param name="userId">Identyfikator właściciela</param>
    /// <param name="page">Numer strony</param>
    /// <param name="pageSize">Rozmiar strony</param>
    /// <param name="sortBy">Pole sortowania</param>
    /// <param name="filterName">Filtr nazwy (częściowe dopasowanie)</param>
    /// <param name="includeArchived">Czy uwzględnić zarchiwizowane leki</param>
    /// <param name="cancellationToken">Token anulowania operacji</param>
    /// <returns>Lista leków i całkowita liczba rekordów</returns>
    Task<(List<Drug> Drugs, int TotalCount)> GetPagedAsync(
        Guid userId,
        int page,
        int pageSize,
        string? sortBy = null,
        string? filterName = null,
        bool includeArchived = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sprawdza czy istnieje lek z danym kodem kreskowym
    /// </summary>
    /// <param name="barcode">Kod kreskowy do sprawdzenia</param>
    /// <param name="excludeDrugId">Identyfikator leku do wykluczenia z sprawdzenia (dla aktualizacji)</param>
    /// <param name="cancellationToken">Token anulowania operacji</param>
    /// <returns>True jeśli kod kreskowy istnieje, false w przeciwnym przypadku</returns>
    Task<bool> ExistsByBarcodeAsync(string barcode, Guid? excludeDrugId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Dodaje nowy lek do repozytorium
    /// </summary>
    /// <param name="drug">Lek do dodania</param>
    /// <param name="cancellationToken">Token anulowania operacji</param>
    Task AddAsync(Drug drug, CancellationToken cancellationToken = default);

    /// <summary>
    /// Aktualizuje istniejący lek
    /// </summary>
    /// <param name="drug">Lek do aktualizacji</param>
    /// <param name="cancellationToken">Token anulowania operacji</param>
    Task UpdateAsync(Drug drug, CancellationToken cancellationToken = default);

    /// <summary>
    /// Zapisuje zmiany do bazy danych
    /// </summary>
    /// <param name="cancellationToken">Token anulowania operacji</param>
    /// <returns>Liczba zmienionych rekordów</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
