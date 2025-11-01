using HomeStorageApp.Drugs.Core.Domain.SystemUnits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeStorageApp.Drugs.Persistence.Configurations;

/// <summary>
/// Konfiguracja Entity Framework dla encji SystemUnit.
/// Definiuje mapowanie, indexy, constrainty i seed data.
/// </summary>
public sealed class SystemUnitConfiguration : IEntityTypeConfiguration<SystemUnit>
{
    /// <summary>
    /// Konfiguruje encję SystemUnit dla bazy danych.
    /// </summary>
    /// <param name="builder">Builder do konfiguracji encji</param>
    public void Configure(EntityTypeBuilder<SystemUnit> builder)
    {
        // Nazwa tabeli
        builder.ToTable("system_units");

        // Klucz główny
        builder.HasKey(su => su.Id);

        // Właściwości
        builder.Property(su => su.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(su => su.Symbol)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(su => su.Category)
            .IsRequired()
            .HasConversion<string>() // Przechowywanie enum jako string
            .HasMaxLength(20);

        // Indexy dla optymalizacji zapytań
        builder.HasIndex(su => su.Name)
            .HasDatabaseName("ix_system_units_name");

        builder.HasIndex(su => su.Symbol)
            .HasDatabaseName("ix_system_units_symbol");

        builder.HasIndex(su => su.Category)
            .HasDatabaseName("ix_system_units_category");

        // Seed data - predefiniowane jednostki systemowe
        SeedSystemUnits(builder);
    }

    /// <summary>
    /// Wypełnia bazę predefiniowanymi jednostkami systemowymi
    /// </summary>
    /// <param name="builder">Builder do konfiguracji encji</param>
    private static void SeedSystemUnits(EntityTypeBuilder<SystemUnit> builder)
    {
        builder.HasData(
            // Jednostki sztukowe (Piece)
            new
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Name = "tabletka",
                Symbol = "tab",
                Category = SystemUnitCategory.Piece
            },
            new
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Name = "kapsułka",
                Symbol = "kaps",
                Category = SystemUnitCategory.Piece
            },
            new
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                Name = "sztuka",
                Symbol = "szt",
                Category = SystemUnitCategory.Piece
            },
            new
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                Name = "dawka",
                Symbol = "daw",
                Category = SystemUnitCategory.Piece
            },
            
            // Jednostki objętości (Volume)
            new
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000011"),
                Name = "mililitr",
                Symbol = "ml",
                Category = SystemUnitCategory.Volume
            },
            new
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000012"),
                Name = "litr",
                Symbol = "l",
                Category = SystemUnitCategory.Volume
            },
            new
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000013"),
                Name = "kropla",
                Symbol = "kr",
                Category = SystemUnitCategory.Volume
            },
            
            // Jednostki masy (Weight)
            new
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000021"),
                Name = "miligram",
                Symbol = "mg",
                Category = SystemUnitCategory.Weight
            },
            new
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000022"),
                Name = "gram",
                Symbol = "g",
                Category = SystemUnitCategory.Weight
            },
            new
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000023"),
                Name = "kilogram",
                Symbol = "kg",
                Category = SystemUnitCategory.Weight
            },
            
            // Inne jednostki (Other)
            new
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000031"),
                Name = "saszetka",
                Symbol = "sasz",
                Category = SystemUnitCategory.Other
            },
            new
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000032"),
                Name = "ampułka",
                Symbol = "amp",
                Category = SystemUnitCategory.Other
            },
            new
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000033"),
                Name = "fiolka",
                Symbol = "fiol",
                Category = SystemUnitCategory.Other
            }
        );
    }
}
