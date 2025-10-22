namespace HomeStorageApp.Identity.Core.Domain.Authentication;

/// <summary>
/// Encja reprezentująca refresh token używany do odświeżania access tokenów JWT.
/// Refresh tokeny mają dłuższy czas życia (7 dni) i mogą być unieważnione przed wygaśnięciem.
/// </summary>
public sealed class RefreshToken
{
    /// <summary>
    /// Unikalny identyfikator refresh tokena
    /// </summary>
    public Guid Id { get; private set; }
    
    /// <summary>
    /// Identyfikator użytkownika, do którego należy token
    /// </summary>
    public Guid UserId { get; private set; }
    
    /// <summary>
    /// Wartość tokena (base64-encoded random bytes)
    /// </summary>
    public string Token { get; private set; } = default!;
    
    /// <summary>
    /// Data i czas wygaśnięcia tokena
    /// </summary>
    public DateTimeOffset ExpiresAt { get; private set; }
    
    /// <summary>
    /// Data i czas utworzenia tokena
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; }
    
    /// <summary>
    /// Określa, czy token został unieważniony (np. podczas wylogowania)
    /// </summary>
    public bool IsRevoked { get; private set; }
    
    /// <summary>
    /// Data i czas unieważnienia tokena (null jeśli token jest aktywny)
    /// </summary>
    public DateTimeOffset? RevokedAt { get; private set; }

    /// <summary>
    /// Właściciel tokena - relacja do encji User
    /// </summary>
    public Users.User User { get; private set; } = default!;

    /// <summary>
    /// Konstruktor prywatny dla Entity Framework
    /// </summary>
    private RefreshToken()
    {
    }

    /// <summary>
    /// Metoda fabryczna tworząca nowy refresh token.
    /// Automatycznie ustawia: nowe ID, czas utworzenia i status IsRevoked na false.
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika</param>
    /// <param name="token">Wartość tokena</param>
    /// <param name="expiresAt">Data i czas wygaśnięcia</param>
    /// <returns>Nowo utworzony obiekt RefreshToken</returns>
    public static RefreshToken Create(Guid userId, string token, DateTimeOffset expiresAt)
    {
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            ExpiresAt = expiresAt,
            CreatedAt = DateTimeOffset.UtcNow,
            IsRevoked = false
        };
    }

    /// <summary>
    /// Unieważnia token, ustawiając IsRevoked na true i zapisując czas unieważnienia.
    /// Używane podczas wylogowania lub wymuszenia ponownego logowania.
    /// </summary>
    public void Revoke()
    {
        IsRevoked = true;
        RevokedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Sprawdza, czy token jest ważny.
    /// Token jest ważny, jeśli nie został unieważniony i nie wygasł.
    /// </summary>
    /// <returns>True jeśli token jest ważny, false w przeciwnym razie</returns>
    public bool IsValid() => !IsRevoked && ExpiresAt > DateTimeOffset.UtcNow;
}
