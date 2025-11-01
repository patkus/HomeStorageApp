namespace HomeStorageApp.Drugs.Core.Application.DTOs;

/// <summary>
/// Request DTO dla tworzenia nowego leku
/// </summary>
public sealed record CreateDrugRequest(
    string Name,
    Guid PrimaryUnitId,
    List<string>? Barcodes,
    List<CreateDerivedUnitDto>? DerivedUnits);
