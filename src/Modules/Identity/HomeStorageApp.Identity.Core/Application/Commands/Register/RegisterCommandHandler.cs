using HomeStorageApp.Identity.Core.Application.Interfaces;
using HomeStorageApp.Identity.Core.Domain.Authentication;
using HomeStorageApp.Identity.Core.Domain.Email;
using HomeStorageApp.Identity.Core.Domain.Errors;
using HomeStorageApp.Identity.Core.Domain.Password;
using HomeStorageApp.Identity.Core.Domain.Users;

namespace HomeStorageApp.Identity.Core.Application.Commands.Register;

/// <summary>
/// Handler dla RegisterCommand - implementuje logikę rejestracji nowego użytkownika.
/// Proces: walidacja → sprawdzenie unikalności email → hashowanie hasła → 
/// utworzenie użytkownika → generowanie tokenów → zapis do bazy.
/// </summary>
/// <param name="userRepository">Repozytorium użytkowników</param>
/// <param name="refreshTokenRepository">Repozytorium refresh tokenów</param>
/// <param name="passwordHasher">Serwis hashowania haseł</param>
/// <param name="tokenGenerator">Serwis generowania tokenów</param>
public sealed class RegisterCommandHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IPasswordHasher passwordHasher,
    ITokenGenerator tokenGenerator)
{
    /// <summary>
    /// Obsługuje żądanie rejestracji nowego użytkownika.
    /// Rzuca wyjątki domenowe w przypadku błędów biznesowych.
    /// </summary>
    /// <param name="command">Command zawierający dane rejestracji</param>
    /// <param name="cancellationToken">Token anulowania operacji</param>
    /// <returns>RegisterResult z danymi użytkownika i tokenami</returns>
    /// <exception cref="UserAlreadyExistsException">Gdy email już istnieje w systemie</exception>
    /// <exception cref="ArgumentException">Gdy walidacja email lub hasła się nie powiedzie</exception>
    public async Task<RegisterResult> HandleAsync(
        RegisterCommand command,
        CancellationToken cancellationToken = default)
    {
        // 1. Walidacja email za pomocą Value Object
        var emailResult = Email.Create(command.Email);
        if (emailResult.IsFailure)
        {
            throw new ArgumentException(emailResult.Error);
        }

        // 2. Walidacja hasła za pomocą Value Object
        var passwordResult = Password.Create(command.Password);
        if (passwordResult.IsFailure)
        {
            throw new ArgumentException(passwordResult.Error);
        }

        // 3. Sprawdzenie czy email już istnieje
        var emailExists = await userRepository.EmailExistsAsync(
            emailResult.Value.Value,
            cancellationToken);

        if (emailExists)
        {
            throw new UserAlreadyExistsException(emailResult.Value.Value);
        }

        // 4. Hashowanie hasła (PBKDF2 z 100k iteracji)
        var passwordHash = passwordHasher.HashPassword(passwordResult.Value.Value);

        // 5. Utworzenie użytkownika
        var user = User.Create(emailResult.Value.Value, passwordHash);

        // 6. Generowanie tokenów
        var accessToken = tokenGenerator.GenerateAccessToken(user.Id, user.Email);
        var refreshTokenValue = tokenGenerator.GenerateRefreshToken();

        // 7. Obliczenie czasów wygaśnięcia (15 min dla access, 7 dni dla refresh)
        var accessTokenExpiresAt = DateTimeOffset.UtcNow.AddMinutes(15);
        var refreshTokenExpiresAt = DateTimeOffset.UtcNow.AddDays(7);

        // 8. Utworzenie refresh tokena
        var refreshToken = RefreshToken.Create(
            user.Id,
            refreshTokenValue,
            refreshTokenExpiresAt);

        // 9. Zapis do bazy danych (w ramach transakcji)
        await userRepository.AddAsync(user, cancellationToken);
        await refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

        // 10. Zwrócenie rezultatu
        return new RegisterResult(
            user.Id,
            user.Email,
            accessToken,
            refreshTokenValue,
            accessTokenExpiresAt,
            refreshTokenExpiresAt);
    }
}
