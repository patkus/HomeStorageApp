namespace HomeStorageApp.Drugs.Core.Application.DTOs;

/// <summary>
/// Response DTO dla szczegółów leku
/// </summary>
public sealed record DrugDetailResponse(
    Guid Id,
    string Name,
    Guid PrimaryUnitId,
    string PrimaryUnitName,
    List<string> Barcodes,
    bool IsArchived,
    List<DerivedUnitDto> DerivedUnits,
    decimal TotalStock,
    DateOnly? NearestExpiryDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
