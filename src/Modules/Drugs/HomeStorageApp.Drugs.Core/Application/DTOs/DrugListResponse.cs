namespace HomeStorageApp.Drugs.Core.Application.DTOs;

/// <summary>
/// Response DTO dla listy leków
/// </summary>
public sealed record DrugListResponse(
    Guid Id,
    string Name,
    string PrimaryUnitName,
    decimal TotalStock,
    DateOnly? NearestExpiryDate,
    int DerivedUnitsCount,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
