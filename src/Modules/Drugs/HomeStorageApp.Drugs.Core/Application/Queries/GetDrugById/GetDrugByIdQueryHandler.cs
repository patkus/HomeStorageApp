using HomeStorageApp.Drugs.Core.Application.DTOs;
using HomeStorageApp.Drugs.Core.Application.Interfaces;
using HomeStorageApp.Drugs.Core.Domain.Exceptions;

namespace HomeStorageApp.Drugs.Core.Application.Queries.GetDrugById;

/// <summary>
/// Handler dla query pobierania szczegółów leku po identyfikatorze
/// </summary>
public sealed class GetDrugByIdQueryHandler(IDrugRepository drugRepository)
{
    /// <summary>
    /// Obsługuje query pobierania szczegółów leku
    /// </summary>
    /// <param name="query">Query pobierania leku</param>
    /// <param name="cancellationToken">Token anulowania operacji</param>
    /// <returns>Szczegóły leku</returns>
    public async Task<DrugDetailResponse> HandleAsync(
        GetDrugByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        // Pobranie leku z bazy
        var drug = await drugRepository.GetByIdAsync(
            query.DrugId,
            query.UserId,
            query.IncludeArchived,
            cancellationToken);
        
        if (drug is null)
        {
            throw new DrugNotFoundException($"Lek o ID {query.DrugId} nie został znaleziony");
        }
        
        // Mapowanie na DrugDetailResponse
        return MapToDetailResponse(drug);
    }
    
    /// <summary>
    /// Mapuje encję Drug na DrugDetailResponse
    /// </summary>
    /// <param name="drug">Encja leku</param>
    /// <returns>DTO szczegółów leku</returns>
    private static DrugDetailResponse MapToDetailResponse(Domain.Drugs.Drug drug)
    {
        var derivedUnits = drug.DerivedUnits.Select(u => new DerivedUnitDto(
            u.Id,
            u.Name,
            u.BaseUnitId,
            GetBaseUnitName(drug, u.BaseUnitId),
            u.ConversionFactor,
            drug.CalculateConversionToMain(u.Id),
            u.IsDefaultPurchaseUnit,
            u.Barcodes.ToList()
        )).ToList();
        
        return new DrugDetailResponse(
            drug.Id,
            drug.Name,
            drug.PrimaryUnitId,
            drug.PrimaryUnit.Name,
            drug.Barcodes.ToList(),
            drug.IsArchived,
            derivedUnits,
            0m, // TotalStock - wymaga integracji z modułem Stocks
            null, // NearestExpiryDate - wymaga integracji z modułem Stocks
            drug.CreatedAt,
            drug.UpdatedAt);
    }
    
    /// <summary>
    /// Pobiera nazwę jednostki bazowej
    /// </summary>
    /// <param name="drug">Encja leku</param>
    /// <param name="baseUnitId">Identyfikator jednostki bazowej</param>
    /// <returns>Nazwa jednostki bazowej</returns>
    private static string GetBaseUnitName(Domain.Drugs.Drug drug, Guid baseUnitId)
    {
        // Sprawdź czy to PrimaryUnit
        if (baseUnitId == drug.PrimaryUnitId)
        {
            return drug.PrimaryUnit.Name;
        }
        
        // Czy to inna DerivedUnit
        var baseUnit = drug.DerivedUnits.FirstOrDefault(u => u.Id == baseUnitId);
        return baseUnit?.Name ?? "Unknown";
    }
}
