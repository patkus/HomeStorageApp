using System.Reflection;
using HomeStorageApp.Identity.Core.Domain.Authentication;
using HomeStorageApp.Identity.Core.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace HomeStorageApp.Identity.Persistence;

/// <summary>
/// DbContext dla modułu Identity.
/// Zarządza encjami User i RefreshToken.
/// </summary>
public sealed class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
    /// <summary>
    /// DbSet dla użytkowników (właścicieli gospodarstw domowych)
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// DbSet dla refresh tokenów używanych do uwierzytelniania
    /// </summary>
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    /// <summary>
    /// Konfiguruje model przy tworzeniu bazy danych.
    /// Automatycznie aplikuje wszystkie Entity Configurations z bieżącego assembly.
    /// </summary>
    /// <param name="modelBuilder">Builder do konfiguracji modelu</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Automatyczne zastosowanie wszystkich konfiguracji z assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
