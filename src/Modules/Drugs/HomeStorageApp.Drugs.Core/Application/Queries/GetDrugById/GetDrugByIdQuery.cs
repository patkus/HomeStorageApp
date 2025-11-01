namespace HomeStorageApp.Drugs.Core.Application.Queries.GetDrugById;

/// <summary>
/// Query do pobierania szczegółów leku po identyfikatorze
/// </summary>
public sealed record GetDrugByIdQuery(
    Guid UserId,
    Guid DrugId,
    bool IncludeArchived = false);
