using HomeStorageApp.Drugs.Core.Domain.Drugs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeStorageApp.Drugs.Persistence.Configurations;

/// <summary>
/// Konfiguracja Entity Framework dla encji DerivedUnit.
/// Definiuje mapowanie, indexy, constrainty i relacje.
/// </summary>
public sealed class DerivedUnitConfiguration : IEntityTypeConfiguration<DerivedUnit>
{
    /// <summary>
    /// Konfiguruje encję DerivedUnit dla bazy danych.
    /// </summary>
    /// <param name="builder">Builder do konfiguracji encji</param>
    public void Configure(EntityTypeBuilder<DerivedUnit> builder)
    {
        // Nazwa tabeli
        builder.ToTable("derived_units");

        // Klucz główny
        builder.HasKey(du => du.Id);

        // Właściwości podstawowe
        builder.Property(du => du.DrugId)
            .IsRequired();

        builder.Property(du => du.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(du => du.BaseUnitId)
            .IsRequired();

        builder.Property(du => du.ConversionFactor)
            .IsRequired()
            .HasPrecision(18, 6); // Precyzja dla decimal

        builder.Property(du => du.IsDefaultPurchaseUnit)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(du => du.CreatedAt)
            .IsRequired();

        // Konfiguracja listy kodów kreskowych jako JSON
        builder.Property<List<string>>("_barcodes")
            .HasColumnName("barcodes")
            .HasColumnType("jsonb")
            .IsRequired();

        // Relacja do Drug jest już zdefiniowana w DrugConfiguration
        // Tu tylko potwierdzamy foreign key
        builder.HasOne(du => du.Drug)
            .WithMany(d => d.DerivedUnits)
            .HasForeignKey(du => du.DrugId);

        // Indexy dla optymalizacji zapytań
        builder.HasIndex(du => du.DrugId)
            .HasDatabaseName("ix_derived_units_drug_id");

        builder.HasIndex(du => du.BaseUnitId)
            .HasDatabaseName("ix_derived_units_base_unit_id");

        builder.HasIndex(du => new { du.DrugId, du.IsDefaultPurchaseUnit })
            .HasDatabaseName("ix_derived_units_drug_id_is_default");


        // Constraint - unikalność nazwy jednostki w ramach danego leku
        builder.HasIndex(du => new { du.DrugId, du.Name })
            .IsUnique()
            .HasDatabaseName("ix_derived_units_drug_id_name_unique");
    }
}
