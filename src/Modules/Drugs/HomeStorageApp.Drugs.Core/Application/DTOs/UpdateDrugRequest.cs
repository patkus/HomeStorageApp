namespace HomeStorageApp.Drugs.Core.Application.DTOs;

/// <summary>
/// Request DTO dla aktualizacji leku
/// </summary>
public sealed record UpdateDrugRequest(
    string? Name,
    List<string>? Barcodes,
    List<UpdateDerivedUnitDto>? DerivedUnits);
