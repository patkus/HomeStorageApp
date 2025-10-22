using System.Text.RegularExpressions;
using HomeStorageApp.Shared;

namespace HomeStorageApp.Identity.Core.Domain.Email;

/// <summary>
/// Value Object reprezentujący adres email z wbudowaną walidacją.
/// Gwarantuje, że każdy obiekt Email zawiera poprawnie sformatowany adres.
/// </summary>
public sealed record Email
{
    /// <summary>
    /// Wartość adresu email (zawsze lowercase)
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Konstruktor prywatny - użyj metody fabrycznej Create
    /// </summary>
    private Email(string value) => Value = value;

    /// <summary>
    /// Tworzy obiekt Email z walidacją formatu.
    /// Konwertuje adres email na małe litery dla spójności.
    /// </summary>
    /// <param name="value">Adres email do zwalidowania</param>
    /// <returns>Result zawierający Email lub komunikat błędu</returns>
    public static Result<Email> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<Email>.Failure("Email nie może być pusty");
        }

        if (!IsValidEmailFormat(value))
        {
            return Result<Email>.Failure("Nieprawidłowy format email");
        }

        // Normalizacja - zawsze konwertujemy email na lowercase
        return Result<Email>.Success(new Email(value.ToLowerInvariant()));
    }

    /// <summary>
    /// Waliduje format adresu email za pomocą wyrażenia regularnego.
    /// Sprawdza podstawową strukturę: tekst@tekst.tekst
    /// </summary>
    /// <param name="email">Adres email do sprawdzenia</param>
    /// <returns>True jeśli format jest poprawny, false w przeciwnym razie</returns>
    private static bool IsValidEmailFormat(string email)
    {
        var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        return regex.IsMatch(email);
    }
}
