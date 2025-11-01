using HomeStorageApp.Drugs.Core.Domain.Drugs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeStorageApp.Drugs.Persistence.Configurations;

/// <summary>
/// Konfiguracja Entity Framework dla encji Drug.
/// Definiuje mapowanie, indexy, constrainty i relacje.
/// </summary>
public sealed class DrugConfiguration : IEntityTypeConfiguration<Drug>
{
    /// <summary>
    /// Konfiguruje encję Drug dla bazy danych.
    /// </summary>
    /// <param name="builder">Builder do konfiguracji encji</param>
    public void Configure(EntityTypeBuilder<Drug> builder)
    {
        // Nazwa tabeli
        builder.ToTable("drugs");

        // Klucz główny
        builder.HasKey(d => d.Id);

        // Właściwości podstawowe
        builder.Property(d => d.UserId)
            .IsRequired();

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.PrimaryUnitId)
            .IsRequired();

        builder.Property(d => d.IsArchived)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(d => d.CreatedAt)
            .IsRequired();

        builder.Property(d => d.UpdatedAt)
            .IsRequired(false); // Nullable

        builder.Property(d => d.ArchivedAt)
            .IsRequired(false); // Nullable

        // Konfiguracja listy kodów kreskowych jako JSON
        builder.Property<List<string>>("_barcodes")
            .HasColumnName("barcodes")
            .HasColumnType("jsonb")
            .IsRequired();

        // Relacja do SystemUnit (many-to-one)
        builder.HasOne(d => d.PrimaryUnit)
            .WithMany()
            .HasForeignKey(d => d.PrimaryUnitId)
            .OnDelete(DeleteBehavior.Restrict); // Nie można usunąć SystemUnit jeśli jest używana

        // Relacja do DerivedUnits (one-to-many)
        builder.HasMany(d => d.DerivedUnits)
            .WithOne(du => du.Drug)
            .HasForeignKey(du => du.DrugId)
            .OnDelete(DeleteBehavior.Cascade); // Usunięcie leku usuwa derived units

        // Backing field dla enkapsulacji kolekcji
        builder.Navigation(d => d.DerivedUnits)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Indexy dla optymalizacji zapytań
        builder.HasIndex(d => d.UserId)
            .HasDatabaseName("ix_drugs_user_id");

        builder.HasIndex(d => d.Name)
            .HasDatabaseName("ix_drugs_name");

        builder.HasIndex(d => d.IsArchived)
            .HasDatabaseName("ix_drugs_is_archived");

        builder.HasIndex(d => d.PrimaryUnitId)
            .HasDatabaseName("ix_drugs_primary_unit_id");

        // Index composite dla często używanych zapytań
        builder.HasIndex(d => new { d.UserId, d.IsArchived })
            .HasDatabaseName("ix_drugs_user_id_is_archived");

    }
}
