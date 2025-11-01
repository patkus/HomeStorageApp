namespace HomeStorageApp.Drugs.Core.Application.DTOs;

/// <summary>
/// Response DTO dla stronicowanej listy
/// </summary>
/// <typeparam name="T">Typ elementu na liście</typeparam>
public sealed record PaginatedResponse<T>(
    List<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
