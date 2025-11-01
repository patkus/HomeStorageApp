using HomeStorageApp.Drugs.Core.Application.DTOs;
using HomeStorageApp.Drugs.Core.Application.Interfaces;

namespace HomeStorageApp.Drugs.Core.Application.Queries.GetDrugs;

/// <summary>
/// Handler dla query pobierania stronicowanej listy leków
/// </summary>
public sealed class GetDrugsQueryHandler(IDrugRepository drugRepository)
{
    /// <summary>
    /// Obsługuje query pobierania stronicowanej listy leków
    /// </summary>
    /// <param name="query">Query pobierania leków</param>
    /// <param name="cancellationToken">Token anulowania operacji</param>
    /// <returns>Stronicowana lista leków</returns>
    public async Task<PaginatedResponse<DrugListResponse>> HandleAsync(
        GetDrugsQuery query,
        CancellationToken cancellationToken = default)
    {
        // Pobranie stronicowanej listy leków
        var (drugs, totalCount) = await drugRepository.GetPagedAsync(
            query.UserId,
            query.Page,
            query.PageSize,
            query.SortBy,
            query.FilterName,
            false, // Nie uwzględniaj zarchiwizowanych
            cancellationToken);
        
        // Mapowanie na DrugListResponse
        var drugListResponses = drugs.Select(drug => new DrugListResponse(
            drug.Id,
            drug.Name,
            drug.PrimaryUnit.Name,
            0m, // TotalStock - wymaga integracji z modułem Stocks
            null, // NearestExpiryDate - wymaga integracji z modułem Stocks
            drug.DerivedUnits.Count,
            drug.CreatedAt,
            drug.UpdatedAt
        )).ToList();
        
        // Obliczenie liczby stron
        var totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);
        
        return new PaginatedResponse<DrugListResponse>(
            drugListResponses,
            query.Page,
            query.PageSize,
            totalCount,
            totalPages);
    }
}
