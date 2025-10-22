using HomeStorageApp.Identity.Core.Domain.Users;

namespace HomeStorageApp.Identity.Core.Application.Interfaces;

/// <summary>
/// Kontrakt dla repozytorium użytkowników.
/// Zapewnia operacje CRUD dla encji User.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Pobiera użytkownika po adresie email.
    /// Email jest unikalnym identyfikatorem używanym do logowania.
    /// </summary>
    /// <param name="email">Adres email użytkownika (case-insensitive)</param>
    /// <param name="cancellationToken">Token anulowania operacji</param>
    /// <returns>Obiekt User jeśli znaleziony, null w przeciwnym razie</returns>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pobiera użytkownika po identyfikatorze wraz z jego refresh tokenami.
    /// Używa eager loading dla optymalizacji.
    /// </summary>
    /// <param name="userId">Unikalny identyfikator użytkownika</param>
    /// <param name="cancellationToken">Token anulowania operacji</param>
    /// <returns>Obiekt User z załadowanymi tokenami jeśli znaleziony, null w przeciwnym razie</returns>
    Task<User?> GetByIdWithTokensAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sprawdza, czy użytkownik o podanym adresie email już istnieje w systemie.
    /// Używane podczas rejestracji dla walidacji unikalności.
    /// </summary>
    /// <param name="email">Adres email do sprawdzenia (case-insensitive)</param>
    /// <param name="cancellationToken">Token anulowania operacji</param>
    /// <returns>True jeśli email istnieje, false w przeciwnym razie</returns>
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Dodaje nowego użytkownika do bazy danych.
    /// Używane podczas procesu rejestracji.
    /// </summary>
    /// <param name="user">Obiekt User do dodania</param>
    /// <param name="cancellationToken">Token anulowania operacji</param>
    Task AddAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Aktualizuje istniejącego użytkownika w bazie danych.
    /// Używane np. do aktualizacji czasu ostatniego logowania.
    /// </summary>
    /// <param name="user">Obiekt User z zaktualizowanymi danymi</param>
    /// <param name="cancellationToken">Token anulowania operacji</param>
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
}
