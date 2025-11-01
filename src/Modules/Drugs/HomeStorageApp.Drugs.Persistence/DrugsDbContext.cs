using System.Reflection;
using HomeStorageApp.Drugs.Core.Domain.Drugs;
using HomeStorageApp.Drugs.Core.Domain.SystemUnits;
using Microsoft.EntityFrameworkCore;

namespace HomeStorageApp.Drugs.Persistence;

/// <summary>
/// DbContext dla modułu Drugs.
/// Zarządza encjami Drug, DerivedUnit i SystemUnit.
/// </summary>
public sealed class DrugsDbContext(DbContextOptions<DrugsDbContext> options) : DbContext(options)
{
    /// <summary>
    /// DbSet dla leków
    /// </summary>
    public DbSet<Drug> Drugs => Set<Drug>();

    /// <summary>
    /// DbSet dla systemowych jednostek miar
    /// </summary>
    public DbSet<SystemUnit> SystemUnits => Set<SystemUnit>();

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
