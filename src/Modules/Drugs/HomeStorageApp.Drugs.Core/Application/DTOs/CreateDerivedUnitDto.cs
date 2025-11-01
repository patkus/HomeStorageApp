namespace HomeStorageApp.Drugs.Core.Application.DTOs;

/// <summary>
/// DTO dla tworzenia jednostki pochodnej
/// </summary>
public sealed record CreateDerivedUnitDto(
    string Name,
    Guid BaseUnitId,
    decimal ConversionFactor,
    bool IsDefaultPurchaseUnit,
    List<string>? Barcodes);
