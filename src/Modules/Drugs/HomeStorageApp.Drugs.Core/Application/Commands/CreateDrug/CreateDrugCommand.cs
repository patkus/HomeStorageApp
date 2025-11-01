using HomeStorageApp.Drugs.Core.Application.DTOs;

namespace HomeStorageApp.Drugs.Core.Application.Commands.CreateDrug;

/// <summary>
/// Command do tworzenia nowego leku
/// </summary>
public sealed record CreateDrugCommand(
    Guid UserId,
    string Name,
    Guid PrimaryUnitId,
    List<string>? Barcodes,
    List<CreateDerivedUnitDto>? DerivedUnits);
