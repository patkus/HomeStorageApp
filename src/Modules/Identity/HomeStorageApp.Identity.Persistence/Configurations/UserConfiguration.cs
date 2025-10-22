using HomeStorageApp.Identity.Core.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeStorageApp.Identity.Persistence.Configurations;

/// <summary>
/// Konfiguracja Entity Framework dla encji User.
/// Definiuje mapowanie, indexy, constrainty i relacje.
/// </summary>
public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    /// <summary>
    /// Konfiguruje encję User dla bazy danych.
    /// </summary>
    /// <param name="builder">Builder do konfiguracji encji</param>
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Nazwa tabeli
        builder.ToTable("Users");

        // Klucz główny
        builder.HasKey(u => u.Id);

        // Właściwości
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.LastLoginAt)
            .IsRequired(false); // Nullable

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Indexy dla optymalizacji zapytań
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email");

        builder.HasIndex(u => u.IsActive)
            .HasDatabaseName("IX_Users_IsActive");

        // Relacja do RefreshTokens (one-to-many)
        builder.HasMany(u => u.RefreshTokens)
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Usunięcie użytkownika usuwa tokeny
    }
}
