using HomeStorageApp.Drugs.Core.Application.DTOs;

namespace HomeStorageApp.Drugs.Core.Application.Commands.UpdateDrug;

/// <summary>
/// Command do aktualizacji leku
/// </summary>
public sealed record UpdateDrugCommand(
    Guid UserId,
    Guid DrugId,
    string? Name,
    List<string>? Barcodes,
    List<UpdateDerivedUnitDto>? DerivedUnits);
