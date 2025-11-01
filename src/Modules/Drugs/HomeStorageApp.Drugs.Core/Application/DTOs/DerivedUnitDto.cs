namespace HomeStorageApp.Drugs.Core.Application.DTOs;

/// <summary>
/// Response DTO dla jednostki pochodnej
/// </summary>
public sealed record DerivedUnitDto(
    Guid Id,
    string Name,
    Guid BaseUnitId,
    string BaseUnitName,
    decimal ConversionFactor,
    decimal ConversionToMain,
    bool IsDefaultPurchaseUnit,
    List<string> Barcodes);
