using HomeStorageApp.Identity.Core.Application.Interfaces;
using HomeStorageApp.Identity.Core.Domain.Authentication;
using HomeStorageApp.Identity.Core.Domain.Errors;

namespace HomeStorageApp.Identity.Core.Application.Commands.Login;

/// <summary>
/// Handler dla LoginCommand - implementuje logikę logowania użytkownika.
/// Proces: weryfikacja credentials → sprawdzenie aktywności konta → 
/// aktualizacja last login → unieważnienie starych tokenów → generowanie nowych tokenów.
/// </summary>
/// <param name="userRepository">Repozytorium użytkowników</param>
/// <param name="refreshTokenRepository">Repozytorium refresh tokenów</param>
/// <param name="passwordHasher">Serwis hashowania haseł</param>
/// <param name="tokenGenerator">Serwis generowania tokenów</param>
public sealed class LoginCommandHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IPasswordHasher passwordHasher,
    ITokenGenerator tokenGenerator)
{
    /// <summary>
    /// Obsługuje żądanie logowania użytkownika.
    /// Rzuca wyjątki domenowe w przypadku błędów biznesowych.
    /// </summary>
    /// <param name="command">Command zawierający dane logowania</param>
    /// <param name="cancellationToken">Token anulowania operacji</param>
    /// <returns>LoginResult z danymi użytkownika i tokenami</returns>
    /// <exception cref="InvalidCredentialsException">Gdy email lub hasło jest nieprawidłowe</exception>
    /// <exception cref="UserNotActiveException">Gdy konto użytkownika jest nieaktywne</exception>
    public async Task<LoginResult> HandleAsync(
        LoginCommand command,
        CancellationToken cancellationToken = default)
    {
        // 1. Pobranie użytkownika po email (case-insensitive)
        var user = await userRepository.GetByEmailAsync(
            command.Email.ToLowerInvariant(),
            cancellationToken);

        // 2. Sprawdzenie czy użytkownik istnieje
        if (user is null)
        {
            throw new InvalidCredentialsException();
        }

        // 3. Weryfikacja hasła (timing-safe comparison)
        var isPasswordValid = passwordHasher.VerifyPassword(
            command.Password,
            user.PasswordHash);

        if (!isPasswordValid)
        {
            throw new InvalidCredentialsException();
        }

        // 4. Sprawdzenie czy konto jest aktywne
        if (!user.IsActive)
        {
            throw new UserNotActiveException();
        }

        // 5. Aktualizacja czasu ostatniego logowania
        user.UpdateLastLogin();

        // 6. Unieważnienie wszystkich starych refresh tokenów użytkownika
        await refreshTokenRepository.RevokeAllForUserAsync(user.Id, cancellationToken);

        // 7. Generowanie nowych tokenów
        var accessToken = tokenGenerator.GenerateAccessToken(user.Id, user.Email);
        var refreshTokenValue = tokenGenerator.GenerateRefreshToken();

        // 8. Obliczenie czasów wygaśnięcia (15 min dla access, 7 dni dla refresh)
        var accessTokenExpiresAt = DateTimeOffset.UtcNow.AddMinutes(15);
        var refreshTokenExpiresAt = DateTimeOffset.UtcNow.AddDays(7);

        // 9. Utworzenie nowego refresh tokena
        var refreshToken = RefreshToken.Create(
            user.Id,
            refreshTokenValue,
            refreshTokenExpiresAt);

        // 10. Zapis zmian do bazy danych (update user + add token)
        await userRepository.UpdateAsync(user, cancellationToken);
        await refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

        // 11. Zwrócenie rezultatu
        return new LoginResult(
            user.Id,
            user.Email,
            accessToken,
            refreshTokenValue,
            accessTokenExpiresAt,
            refreshTokenExpiresAt);
    }
}
