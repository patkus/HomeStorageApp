namespace HomeStorageApp.Identity.Core.Application.Interfaces;

/// <summary>
/// Kontrakt dla serwisu hashowania i weryfikacji haseł.
/// Implementacja używa algorytmu PBKDF2 z 100,000 iteracji dla bezpieczeństwa.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashuje hasło w formie plaintext używając bezpiecznego algorytmu (PBKDF2).
    /// Generuje losowy salt i wykonuje 100,000 iteracji SHA256.
    /// </summary>
    /// <param name="password">Hasło w formie plaintext do zahashowania</param>
    /// <returns>Zhashowane hasło w formacie: iteracje.salt.hash (base64)</returns>
    string HashPassword(string password);

    /// <summary>
    /// Weryfikuje, czy podane hasło plaintext pasuje do zahashowanego hasła.
    /// Używa timing-safe comparison aby zapobiec timing attacks.
    /// </summary>
    /// <param name="password">Hasło plaintext do weryfikacji</param>
    /// <param name="passwordHash">Zhashowane hasło z bazy danych</param>
    /// <returns>True jeśli hasła pasują, false w przeciwnym razie</returns>
    bool VerifyPassword(string password, string passwordHash);
}
