namespace HomeStorageApp.Drugs.Core.Application.Commands.ArchiveDrug;

/// <summary>
/// Command do archiwizacji leku
/// </summary>
public sealed record ArchiveDrugCommand(
    Guid UserId,
    Guid DrugId);
