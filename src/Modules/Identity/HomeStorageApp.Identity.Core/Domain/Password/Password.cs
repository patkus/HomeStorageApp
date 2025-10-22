using HomeStorageApp.Shared;

namespace HomeStorageApp.Identity.Core.Domain.Password;

/// <summary>
/// Value Object reprezentujący hasło z kompleksową walidacją wymagań bezpieczeństwa.
/// Wymusza politykę haseł: min 8 znaków, wielka litera, mała litera, cyfra, znak specjalny.
/// </summary>
public sealed record Password
{
    /// <summary>
    /// Wartość hasła w formie plaintext (przed hashowaniem)
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Konstruktor prywatny - użyj metody fabrycznej Create
    /// </summary>
    private Password(string value) => Value = value;

    /// <summary>
    /// Tworzy obiekt Password z pełną walidacją wymagań bezpieczeństwa.
    /// Sprawdza długość, obecność wielkich/małych liter, cyfr i znaków specjalnych.
    /// </summary>
    /// <param name="value">Hasło do zwalidowania</param>
    /// <returns>Result zawierający Password lub szczegółowy komunikat błędu</returns>
    public static Result<Password> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<Password>.Failure("Hasło nie może być puste");
        }

        if (value.Length < 8)
        {
            return Result<Password>.Failure("Hasło musi mieć minimum 8 znaków");
        }

        if (!HasUpperCase(value))
        {
            return Result<Password>.Failure("Hasło musi zawierać co najmniej jedną wielką literę");
        }

        if (!HasLowerCase(value))
        {
            return Result<Password>.Failure("Hasło musi zawierać co najmniej jedną małą literę");
        }

        if (!HasDigit(value))
        {
            return Result<Password>.Failure("Hasło musi zawierać co najmniej jedną cyfrę");
        }

        if (!HasSpecialChar(value))
        {
            return Result<Password>.Failure("Hasło musi zawierać co najmniej jeden znak specjalny");
        }

        return Result<Password>.Success(new Password(value));
    }

    /// <summary>
    /// Sprawdza, czy hasło zawiera przynajmniej jedną wielką literę
    /// </summary>
    private static bool HasUpperCase(string value) => value.Any(char.IsUpper);
    
    /// <summary>
    /// Sprawdza, czy hasło zawiera przynajmniej jedną małą literę
    /// </summary>
    private static bool HasLowerCase(string value) => value.Any(char.IsLower);
    
    /// <summary>
    /// Sprawdza, czy hasło zawiera przynajmniej jedną cyfrę
    /// </summary>
    private static bool HasDigit(string value) => value.Any(char.IsDigit);
    
    /// <summary>
    /// Sprawdza, czy hasło zawiera przynajmniej jeden znak specjalny (nie-alfanumeryczny)
    /// </summary>
    private static bool HasSpecialChar(string value) => value.Any(c => !char.IsLetterOrDigit(c));
}
