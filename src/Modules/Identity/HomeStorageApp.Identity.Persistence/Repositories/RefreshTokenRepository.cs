using HomeStorageApp.Identity.Core.Application.Interfaces;
using HomeStorageApp.Identity.Core.Domain.Authentication;
using Microsoft.EntityFrameworkCore;

namespace HomeStorageApp.Identity.Persistence.Repositories;

/// <summary>
/// Implementacja repozytorium dla encji RefreshToken.
/// Obsługuje zarządzanie cyklem życia refresh tokenów.
/// </summary>
public sealed class RefreshTokenRepository(IdentityDbContext context) : IRefreshTokenRepository
{
    /// <summary>
    /// Pobiera refresh token po jego wartości.
    /// </summary>
    /// <param name="token">Wartość tokena do wyszukania</param>
    /// <param name="cancellationToken">Token anulowania operacji</param>
    /// <returns>RefreshToken lub null jeśli nie znaleziono</returns>
    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
    }

    /// <summary>
    /// Unieważnia wszystkie aktywne refresh tokeny dla danego użytkownika.
    /// Używane podczas logowania (czyszczenie starych sesji) lub zmiany hasła.
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika</param>
    /// <param name="cancellationToken">Token anulowania operacji</param>
    public async Task RevokeAllForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // Pobierz wszystkie aktywne tokeny użytkownika
        var activeTokens = await context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync(cancellationToken);

        // Unieważnij każdy token
        foreach (var token in activeTokens)
        {
            token.Revoke();
        }

        // Zapisz zmiany
        await context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Dodaje nowy refresh token do bazy danych.
    /// </summary>
    /// <param name="refreshToken">Token do dodania</param>
    /// <param name="cancellationToken">Token anulowania operacji</param>
    public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        await context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Aktualizuje istniejący refresh token w bazie danych.
    /// Używane podczas unieważniania tokena (wylogowanie).
    /// </summary>
    /// <param name="refreshToken">Token do aktualizacji</param>
    /// <param name="cancellationToken">Token anulowania operacji</param>
    public async Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        context.RefreshTokens.Update(refreshToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
