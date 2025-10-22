using HomeStorageApp.Identity.Core.Domain.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeStorageApp.Identity.Persistence.Configurations;

/// <summary>
/// Konfiguracja Entity Framework dla encji RefreshToken.
/// Definiuje mapowanie, indexy, constrainty i relacje.
/// </summary>
public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    /// <summary>
    /// Konfiguruje encję RefreshToken dla bazy danych.
    /// </summary>
    /// <param name="builder">Builder do konfiguracji encji</param>
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        // Nazwa tabeli
        builder.ToTable("RefreshTokens");

        // Klucz główny
        builder.HasKey(rt => rt.Id);

        // Właściwości
        builder.Property(rt => rt.UserId)
            .IsRequired();

        builder.Property(rt => rt.Token)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(rt => rt.ExpiresAt)
            .IsRequired();

        builder.Property(rt => rt.CreatedAt)
            .IsRequired();

        builder.Property(rt => rt.IsRevoked)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(rt => rt.RevokedAt)
            .IsRequired(false); // Nullable

        // Indexy dla optymalizacji zapytań
        builder.HasIndex(rt => rt.Token)
            .IsUnique()
            .HasDatabaseName("IX_RefreshTokens_Token");

        builder.HasIndex(rt => rt.UserId)
            .HasDatabaseName("IX_RefreshTokens_UserId");

        // Index kompozytowy dla filtrowania aktywnych tokenów
        builder.HasIndex(rt => new { rt.ExpiresAt, rt.IsRevoked })
            .HasDatabaseName("IX_RefreshTokens_ExpiresAt_IsRevoked")
            .HasFilter("\"IsRevoked\" = false");

        // Relacja do User (many-to-one)
        builder.HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
