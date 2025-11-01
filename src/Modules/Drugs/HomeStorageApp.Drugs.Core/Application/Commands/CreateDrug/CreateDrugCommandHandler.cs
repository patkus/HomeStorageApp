using HomeStorageApp.Drugs.Core.Application.DTOs;
using HomeStorageApp.Drugs.Core.Application.Interfaces;
using HomeStorageApp.Drugs.Core.Domain.Drugs;
using HomeStorageApp.Drugs.Core.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace HomeStorageApp.Drugs.Core.Application.Commands.CreateDrug;

/// <summary>
/// Handler dla komendy tworzenia leku
/// </summary>
public sealed class CreateDrugCommandHandler(
    IDrugRepository drugRepository,
    ISystemUnitRepository systemUnitRepository,
    ILogger<CreateDrugCommandHandler> logger)
{
    /// <summary>
    /// Obsługuje komendę tworzenia nowego leku
    /// </summary>
    /// <param name="command">Komenda tworzenia leku</param>
    /// <param name="cancellationToken">Token anulowania operacji</param>
    /// <returns>Szczegóły utworzonego leku</returns>
    public async Task<DrugDetailResponse> HandleAsync(
        CreateDrugCommand command,
        CancellationToken cancellationToken = default)
    {
        // Walidacja istnienia SystemUnit
        var systemUnit = await systemUnitRepository.GetByIdAsync(command.PrimaryUnitId, cancellationToken);
        if (systemUnit is null)
        {
            throw new DrugValidationException(nameof(command.PrimaryUnitId), "Wybrana jednostka główna nie istnieje");
        }
        
        // Sprawdzenie unikalności barcode dla leku
        if (command.Barcodes is not null)
        {
            foreach (var barcode in command.Barcodes)
            {
                var exists = await drugRepository.ExistsByBarcodeAsync(barcode, null, cancellationToken);
                if (exists)
                {
                    throw new DrugConflictException($"Lek z kodem kreskowym '{barcode}' już istnieje");
                }
            }
        }
        
        // Utworzenie encji Drug
        var drug = Drug.Create(command.UserId, command.Name, command.PrimaryUnitId, command.Barcodes);
        
        // Dodanie jednostek pochodnych
        if (command.DerivedUnits is not null)
        {
            foreach (var unitDto in command.DerivedUnits)
            {
                // Walidacja BaseUnit (może być PrimaryUnit lub inna DerivedUnit)
                if (unitDto.BaseUnitId != command.PrimaryUnitId)
                {
                    // BaseUnit to inna DerivedUnit - zostanie zwalidowane przy zapisie
                    // Tutaj tylko sprawdzamy czy ConversionFactor > 0
                    if (unitDto.ConversionFactor <= 0)
                    {
                        throw new DrugValidationException(
                            nameof(unitDto.ConversionFactor),
                            "Przelicznik musi być większy od 0");
                    }
                }
                
                // Sprawdzenie unikalności barcode dla jednostki pochodnej
                if (unitDto.Barcodes is not null)
                {
                    foreach (var barcode in unitDto.Barcodes)
                    {
                        var exists = await drugRepository.ExistsByBarcodeAsync(barcode, null, cancellationToken);
                        if (exists)
                        {
                            throw new DrugConflictException($"Kod kreskowy '{barcode}' jest już używany");
                        }
                    }
                }
                
                drug.AddDerivedUnit(
                    unitDto.Name,
                    unitDto.BaseUnitId,
                    unitDto.ConversionFactor,
                    unitDto.IsDefaultPurchaseUnit,
                    unitDto.Barcodes);
            }
        }
        
        // Zapis do bazy
        await drugRepository.AddAsync(drug, cancellationToken);
        await drugRepository.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation(
            "Drug {DrugId} created by user {UserId} with name {DrugName}",
            drug.Id, command.UserId, drug.Name);
        
        // Ponowne pobranie z Include dla navigation properties
        var createdDrug = await drugRepository.GetByIdAsync(drug.Id, command.UserId, true, cancellationToken);
        if (createdDrug is null)
        {
            throw new DrugNotFoundException($"Nie można pobrać utworzonego leku o ID {drug.Id}");
        }
        
        // Zwrócenie response
        return MapToDetailResponse(createdDrug);
    }
    
    /// <summary>
    /// Mapuje encję Drug na DrugDetailResponse
    /// </summary>
    /// <param name="drug">Encja leku</param>
    /// <returns>DTO szczegółów leku</returns>
    private static DrugDetailResponse MapToDetailResponse(Drug drug)
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
    private static string GetBaseUnitName(Drug drug, Guid baseUnitId)
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
