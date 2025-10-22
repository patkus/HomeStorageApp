using HomeStorageApp.Identity.Core.Application.Interfaces;
using HomeStorageApp.Identity.Core.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace HomeStorageApp.Identity.Persistence.Repositories;

/// <summary>
/// Implementacja repozytorium dla encji User.
/// Obsługuje operacje CRUD na użytkownikach z optymalizacją zapytań.
/// </summary>
public sealed class UserRepository(IdentityDbContext context) : IUserRepository
{
    /// <summary>
    /// Pobiera użytkownika po adresie email (case-insensitive).
    /// </summary>
    /// <param name="email">Adres email użytkownika</param>
    /// <param name="cancellationToken">Token anulowania operacji</param>
    /// <returns>Użytkownik lub null jeśli nie znaleziono</returns>
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .FirstOrDefaultAsync(u => u.Email == email.ToLower(), cancellationToken);
    }

    /// <summary>
    /// Pobiera użytkownika po ID wraz z jego refresh tokenami (eager loading).
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika</param>
    /// <param name="cancellationToken">Token anulowania operacji</param>
    /// <returns>Użytkownik z tokenami lub null jeśli nie znaleziono</returns>
    public async Task<User?> GetByIdWithTokensAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .Include(u => u.RefreshTokens.Where(rt => !rt.IsRevoked))
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    /// <summary>
    /// Sprawdza czy użytkownik o podanym email już istnieje (optymalizacja - tylko Count).
    /// </summary>
    /// <param name="email">Adres email do sprawdzenia</param>
    /// <param name="cancellationToken">Token anulowania operacji</param>
    /// <returns>True jeśli email istnieje, false w przeciwnym razie</returns>
    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .AnyAsync(u => u.Email == email.ToLower(), cancellationToken);
    }

    /// <summary>
    /// Dodaje nowego użytkownika do bazy danych.
    /// </summary>
    /// <param name="user">Użytkownik do dodania</param>
    /// <param name="cancellationToken">Token anulowania operacji</param>
    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await context.Users.AddAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Aktualizuje istniejącego użytkownika w bazie danych.
    /// </summary>
    /// <param name="user">Użytkownik do aktualizacji</param>
    /// <param name="cancellationToken">Token anulowania operacji</param>
    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);
    }
}
