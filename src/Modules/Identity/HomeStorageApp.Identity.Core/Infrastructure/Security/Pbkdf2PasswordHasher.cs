using System.Security.Cryptography;
using HomeStorageApp.Identity.Core.Application.Interfaces;

namespace HomeStorageApp.Identity.Core.Infrastructure.Security;

/// <summary>
/// Implementacja hashowania haseł używając algorytmu PBKDF2 (Password-Based Key Derivation Function 2).
/// Zapewnia wysoki poziom bezpieczeństwa z 100,000 iteracji i SHA256.
/// </summary>
public sealed class Pbkdf2PasswordHasher : IPasswordHasher
{
    private const int Iterations = 100_000; // OWASP zaleca minimum 100,000 dla PBKDF2-SHA256
    private const int SaltSize = 32; // 256 bits
    private const int HashSize = 32; // 256 bits

    /// <summary>
    /// Hashuje hasło używając PBKDF2 z losowym salt.
    /// Format: iterations.salt.hash (wszystkie w base64)
    /// </summary>
    /// <param name="password">Hasło plaintext do zahashowania</param>
    /// <returns>Zhashowane hasło w formacie: iterations.salt.hash</returns>
    public string HashPassword(string password)
    {
        // Generuj losowy salt
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[SaltSize];
        rng.GetBytes(salt);

        // Hashuj hasło z salt używając PBKDF2
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password: password,
            salt: salt,
            iterations: Iterations,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: HashSize
        );

        // Zwróć w formacie: iterations.salt.hash (base64)
        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    /// <summary>
    /// Weryfikuje hasło plaintext z zahashowanym hasłem.
    /// Używa timing-safe comparison aby zapobiec timing attacks.
    /// </summary>
    /// <param name="password">Hasło plaintext do zweryfikowania</param>
    /// <param name="passwordHash">Zhashowane hasło z bazy danych</param>
    /// <returns>True jeśli hasła pasują, false w przeciwnym razie</returns>
    public bool VerifyPassword(string password, string passwordHash)
    {
        // Parse format: iterations.salt.hash
        var parts = passwordHash.Split('.');
        if (parts.Length != 3)
        {
            return false;
        }

        // Ekstraktuj parametry
        var iterations = int.Parse(parts[0]);
        var salt = Convert.FromBase64String(parts[1]);
        var hash = Convert.FromBase64String(parts[2]);

        // Hashuj podane hasło z tym samym salt i iteracjami
        var hashToVerify = Rfc2898DeriveBytes.Pbkdf2(
            password: password,
            salt: salt,
            iterations: iterations,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: hash.Length
        );

        // Porównaj hashe używając timing-safe comparison
        return CryptographicOperations.FixedTimeEquals(hash, hashToVerify);
    }
}
