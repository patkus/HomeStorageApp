using HomeStorageApp.Drugs.Core.Application.Interfaces;
using HomeStorageApp.Drugs.Core.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace HomeStorageApp.Drugs.Core.Application.Commands.ArchiveDrug;

/// <summary>
/// Handler dla komendy archiwizacji leku
/// </summary>
public sealed class ArchiveDrugCommandHandler(
    IDrugRepository drugRepository,
    ILogger<ArchiveDrugCommandHandler> logger)
{
    /// <summary>
    /// Obsługuje komendę archiwizacji leku (soft delete)
    /// </summary>
    /// <param name="command">Komenda archiwizacji leku</param>
    /// <param name="cancellationToken">Token anulowania operacji</param>
    public async Task HandleAsync(
        ArchiveDrugCommand command,
        CancellationToken cancellationToken = default)
    {
        // Pobranie istniejącego leku
        var drug = await drugRepository.GetByIdAsync(command.DrugId, command.UserId, false, cancellationToken);
        if (drug is null)
        {
            throw new DrugNotFoundException($"Lek o ID {command.DrugId} nie został znaleziony");
        }
        
        // Sprawdzenie czy lek nie jest już zarchiwizowany
        if (drug.IsArchived)
        {
            throw new DrugConflictException("Lek jest już zarchiwizowany");
        }
        
        // Archiwizacja leku
        drug.Archive();
        
        // Zapis zmian
        await drugRepository.UpdateAsync(drug, cancellationToken);
        await drugRepository.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation(
            "Drug {DrugId} archived by user {UserId}",
            drug.Id, command.UserId);
    }
}
