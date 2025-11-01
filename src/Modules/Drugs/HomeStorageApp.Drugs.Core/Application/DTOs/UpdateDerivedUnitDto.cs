namespace HomeStorageApp.Drugs.Core.Application.DTOs;

/// <summary>
/// DTO dla aktualizacji jednostki pochodnej
/// </summary>
public sealed record UpdateDerivedUnitDto(
    Guid? Id,
    string Name,
    Guid BaseUnitId,
    decimal ConversionFactor,
    bool IsDefaultPurchaseUnit,
    List<string>? Barcodes);
