using HomeStorageApp.Drugs.Core.Application.DTOs;
using HomeStorageApp.Drugs.Core.Application.Interfaces;
using HomeStorageApp.Drugs.Core.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace HomeStorageApp.Drugs.Core.Application.Commands.RestoreDrug;

/// <summary>
/// Handler dla komendy przywracania zarchiwizowanego leku
/// </summary>
public sealed class RestoreDrugCommandHandler(
    IDrugRepository drugRepository,
    ILogger<RestoreDrugCommandHandler> logger)
{
    /// <summary>
    /// Obsługuje komendę przywracania zarchiwizowanego leku
    /// </summary>
    /// <param name="command">Komenda przywracania leku</param>
    /// <param name="cancellationToken">Token anulowania operacji</param>
    /// <returns>Szczegóły przywróconego leku</returns>
    public async Task<DrugDetailResponse> HandleAsync(
        RestoreDrugCommand command,
        CancellationToken cancellationToken = default)
    {
        // Pobranie zarchiwizowanego leku
        var drug = await drugRepository.GetByIdAsync(command.DrugId, command.UserId, true, cancellationToken);
        if (drug is null)
        {
            throw new DrugNotFoundException($"Lek o ID {command.DrugId} nie został znaleziony");
        }
        
        // Sprawdzenie czy lek jest zarchiwizowany
        if (!drug.IsArchived)
        {
            throw new DrugConflictException("Lek nie jest zarchiwizowany");
        }
        
        // Przywrócenie leku
        drug.Restore();
        
        // Zapis zmian
        await drugRepository.UpdateAsync(drug, cancellationToken);
        await drugRepository.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation(
            "Drug {DrugId} restored by user {UserId}",
            drug.Id, command.UserId);
        
        // Ponowne pobranie z Include dla navigation properties
        var restoredDrug = await drugRepository.GetByIdAsync(drug.Id, command.UserId, false, cancellationToken);
        if (restoredDrug is null)
        {
            throw new DrugNotFoundException($"Nie można pobrać przywróconego leku o ID {drug.Id}");
        }
        
        // Zwrócenie response
        return MapToDetailResponse(restoredDrug);
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
