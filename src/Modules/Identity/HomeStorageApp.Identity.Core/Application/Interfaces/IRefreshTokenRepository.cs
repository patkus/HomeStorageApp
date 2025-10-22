using HomeStorageApp.Identity.Core.Domain.Authentication;

namespace HomeStorageApp.Identity.Core.Application.Interfaces;

/// <summary>
/// Kontrakt dla repozytorium refresh tokenów.
/// Zarządza cyklem życia tokenów: tworzenie, walidacja, unieważnianie.
/// </summary>
public interface IRefreshTokenRepository
{
    /// <summary>
    /// Pobiera refresh token po jego wartości.
    /// Używane podczas walidacji tokena przy odświeżaniu access tokena lub wylogowaniu.
    /// </summary>
    /// <param name="token">Wartość refresh tokena</param>
    /// <param name="cancellationToken">Token anulowania operacji</param>
    /// <returns>Obiekt RefreshToken jeśli znaleziony, null w przeciwnym razie</returns>
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unieważnia wszystkie aktywne refresh tokeny dla danego użytkownika.
    /// Używane podczas logowania (aby unieważnić stare sesje) lub zmiany hasła.
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika</param>
    /// <param name="cancellationToken">Token anulowania operacji</param>
    Task RevokeAllForUserAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Dodaje nowy refresh token do bazy danych.
    /// Używane podczas rejestracji i logowania.
    /// </summary>
    /// <param name="refreshToken">Obiekt RefreshToken do dodania</param>
    /// <param name="cancellationToken">Token anulowania operacji</param>
    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Aktualizuje istniejący refresh token w bazie danych.
    /// Używane podczas unieważniania tokena (wylogowanie).
    /// </summary>
    /// <param name="refreshToken">Obiekt RefreshToken z zaktualizowanymi danymi</param>
    /// <param name="cancellationToken">Token anulowania operacji</param>
    Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
}
