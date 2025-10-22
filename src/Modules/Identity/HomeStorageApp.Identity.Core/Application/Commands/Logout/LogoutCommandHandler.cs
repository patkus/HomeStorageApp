using HomeStorageApp.Identity.Core.Application.Interfaces;
using HomeStorageApp.Identity.Core.Domain.Errors;

namespace HomeStorageApp.Identity.Core.Application.Commands.Logout;

/// <summary>
/// Handler dla LogoutCommand - implementuje logikę wylogowania użytkownika.
/// Proces: weryfikacja tokena → sprawdzenie własności → unieważnienie tokena.
/// </summary>
/// <param name="refreshTokenRepository">Repozytorium refresh tokenów</param>
public sealed class LogoutCommandHandler(IRefreshTokenRepository refreshTokenRepository)
{
    /// <summary>
    /// Obsługuje żądanie wylogowania użytkownika poprzez unieważnienie refresh tokena.
    /// Rzuca wyjątki domenowe w przypadku błędów biznesowych.
    /// </summary>
    /// <param name="command">Command zawierający userId i refreshToken</param>
    /// <param name="cancellationToken">Token anulowania operacji</param>
    /// <returns>LogoutResult z informacją o sukcesie</returns>
    /// <exception cref="InvalidTokenException">Gdy token jest nieprawidłowy, wygasł lub nie należy do użytkownika</exception>
    public async Task<LogoutResult> HandleAsync(
        LogoutCommand command,
        CancellationToken cancellationToken = default)
    {
        // 1. Pobranie refresh tokena z bazy danych
        var refreshToken = await refreshTokenRepository.GetByTokenAsync(
            command.RefreshToken,
            cancellationToken);

        // 2. Sprawdzenie czy token istnieje
        if (refreshToken is null)
        {
            throw new InvalidTokenException();
        }

        // 3. Sprawdzenie czy token należy do użytkownika
        if (refreshToken.UserId != command.UserId)
        {
            throw new InvalidTokenException();
        }

        // 4. Sprawdzenie czy token nie jest już unieważniony
        if (refreshToken.IsRevoked)
        {
            throw new InvalidTokenException();
        }

        // 5. Sprawdzenie czy token nie wygasł
        if (refreshToken.ExpiresAt <= DateTimeOffset.UtcNow)
        {
            throw new InvalidTokenException();
        }

        // 6. Unieważnienie tokena
        refreshToken.Revoke();

        // 7. Zapis zmian do bazy danych
        await refreshTokenRepository.UpdateAsync(refreshToken, cancellationToken);

        // 8. Zwrócenie rezultatu
        return new LogoutResult(Success: true);
    }
}
