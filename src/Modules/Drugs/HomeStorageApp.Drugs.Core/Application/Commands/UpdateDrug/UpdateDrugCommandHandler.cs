using HomeStorageApp.Drugs.Core.Application.DTOs;
using HomeStorageApp.Drugs.Core.Application.Interfaces;
using HomeStorageApp.Drugs.Core.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace HomeStorageApp.Drugs.Core.Application.Commands.UpdateDrug;

/// <summary>
/// Handler dla komendy aktualizacji leku
/// </summary>
public sealed class UpdateDrugCommandHandler(
    IDrugRepository drugRepository,
    ILogger<UpdateDrugCommandHandler> logger)
{
    /// <summary>
    /// Obsługuje komendę aktualizacji leku
    /// </summary>
    /// <param name="command">Komenda aktualizacji leku</param>
    /// <param name="cancellationToken">Token anulowania operacji</param>
    /// <returns>Zaktualizowane szczegóły leku</returns>
    public async Task<DrugDetailResponse> HandleAsync(
        UpdateDrugCommand command,
        CancellationToken cancellationToken = default)
    {
        // Pobranie istniejącego leku
        var drug = await drugRepository.GetByIdAsync(command.DrugId, command.UserId, false, cancellationToken);
        if (drug is null)
        {
            throw new DrugNotFoundException($"Lek o ID {command.DrugId} nie został znaleziony");
        }
        
        // Sprawdzenie unikalności barcode
        if (command.Barcodes is not null)
        {
            foreach (var barcode in command.Barcodes)
            {
                var exists = await drugRepository.ExistsByBarcodeAsync(barcode, command.DrugId, cancellationToken);
                if (exists)
                {
                    throw new DrugConflictException($"Kod kreskowy '{barcode}' jest już używany przez inny lek");
                }
            }
        }
        
        // Aktualizacja podstawowych informacji
        drug.Update(command.Name, command.Barcodes);
        
        // Zarządzanie jednostkami pochodnymi
        if (command.DerivedUnits is not null)
        {
            // Pobierz istniejące jednostki ID
            var existingUnitIds = drug.DerivedUnits.Select(u => u.Id).ToHashSet();
            var updatedUnitIds = command.DerivedUnits
                .Where(u => u.Id.HasValue)
                .Select(u => u.Id!.Value)
                .ToHashSet();
            
            // Usuń jednostki które nie są w request (zostały usunięte)
            var unitsToRemove = existingUnitIds.Except(updatedUnitIds).ToList();
            foreach (var unitId in unitsToRemove)
            {
                drug.RemoveDerivedUnit(unitId);
            }
            
            // Aktualizuj lub dodaj jednostki
            foreach (var unitDto in command.DerivedUnits)
            {
                // Walidacja ConversionFactor
                if (unitDto.ConversionFactor <= 0)
                {
                    throw new DrugValidationException(
                        nameof(unitDto.ConversionFactor),
                        "Przelicznik musi być większy od 0");
                }
                
                // Sprawdzenie unikalności barcode dla jednostki pochodnej
                if (unitDto.Barcodes is not null)
                {
                    foreach (var barcode in unitDto.Barcodes)
                    {
                        var exists = await drugRepository.ExistsByBarcodeAsync(barcode, command.DrugId, cancellationToken);
                        if (exists)
                        {
                            throw new DrugConflictException($"Kod kreskowy '{barcode}' jest już używany");
                        }
                    }
                }
                
                if (unitDto.Id.HasValue)
                {
                    // Aktualizuj istniejącą jednostkę
                    drug.UpdateDerivedUnit(
                        unitDto.Id.Value,
                        unitDto.Name,
                        unitDto.ConversionFactor,
                        unitDto.IsDefaultPurchaseUnit,
                        unitDto.Barcodes);
                }
                else
                {
                    // Dodaj nową jednostkę
                    drug.AddDerivedUnit(
                        unitDto.Name,
                        unitDto.BaseUnitId,
                        unitDto.ConversionFactor,
                        unitDto.IsDefaultPurchaseUnit,
                        unitDto.Barcodes);
                }
            }
        }
        
        // Zapis zmian
        await drugRepository.UpdateAsync(drug, cancellationToken);
        await drugRepository.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation(
            "Drug {DrugId} updated by user {UserId}",
            drug.Id, command.UserId);
        
        // Ponowne pobranie z Include dla navigation properties
        var updatedDrug = await drugRepository.GetByIdAsync(drug.Id, command.UserId, false, cancellationToken);
        if (updatedDrug is null)
        {
            throw new DrugNotFoundException($"Nie można pobrać zaktualizowanego leku o ID {drug.Id}");
        }
        
        // Zwrócenie response
        return MapToDetailResponse(updatedDrug);
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
