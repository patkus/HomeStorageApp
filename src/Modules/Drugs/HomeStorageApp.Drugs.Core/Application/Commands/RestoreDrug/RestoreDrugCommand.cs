namespace HomeStorageApp.Drugs.Core.Application.Commands.RestoreDrug;

/// <summary>
/// Command do przywracania zarchiwizowanego leku
/// </summary>
public sealed record RestoreDrugCommand(
    Guid UserId,
    Guid DrugId);
