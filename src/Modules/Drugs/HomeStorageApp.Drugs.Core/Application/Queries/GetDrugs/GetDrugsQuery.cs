namespace HomeStorageApp.Drugs.Core.Application.Queries.GetDrugs;

/// <summary>
/// Query do pobierania stronicowanej listy leków
/// </summary>
public sealed record GetDrugsQuery(
    Guid UserId,
    int Page,
    int PageSize,
    string? SortBy,
    string? FilterName);
