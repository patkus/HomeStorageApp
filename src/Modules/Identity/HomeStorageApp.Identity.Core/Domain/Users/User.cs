namespace HomeStorageApp.Identity.Core.Domain.Users;

/// <summary>
/// Encja reprezentująca użytkownika systemu (właściciela gospodarstwa domowego).
/// Każde gospodarstwo ma jednego właściciela, który jest głównym kontem uwierzytelniającym.
/// </summary>
public sealed class User
{
    /// <summary>
    /// Unikalny identyfikator użytkownika
    /// </summary>
    public Guid Id { get; private set; }
    
    /// <summary>
    /// Adres email użytkownika (unikalny w systemie, używany do logowania)
    /// </summary>
    public string Email { get; private set; } = default!;
    
    /// <summary>
    /// Zhashowane hasło użytkownika (PBKDF2 z 100k iteracji)
    /// </summary>
    public string PasswordHash { get; private set; } = default!;
    
    /// <summary>
    /// Data i czas utworzenia konta użytkownika
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; }
    
    /// <summary>
    /// Data i czas ostatniego logowania (null jeśli nigdy się nie zalogował)
    /// </summary>
    public DateTimeOffset? LastLoginAt { get; private set; }
    
    /// <summary>
    /// Określa, czy konto użytkownika jest aktywne
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Kolekcja refresh tokenów przypisanych do użytkownika
    /// </summary>
    public ICollection<Authentication.RefreshToken> RefreshTokens { get; private set; } = default!;

    /// <summary>
    /// Konstruktor prywatny dla Entity Framework
    /// </summary>
    private User()
    {
    }

    /// <summary>
    /// Metoda fabryczna tworząca nowego użytkownika z domyślnymi wartościami.
    /// Automatycznie ustawia: nowe ID, aktywność na true, pustą kolekcję tokenów i czas utworzenia.
    /// </summary>
    /// <param name="email">Adres email użytkownika</param>
    /// <param name="passwordHash">Zhashowane hasło użytkownika</param>
    /// <returns>Nowo utworzony obiekt User</returns>
    public static User Create(string email, string passwordHash)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = passwordHash,
            CreatedAt = DateTimeOffset.UtcNow,
            IsActive = true,
            RefreshTokens = new List<Authentication.RefreshToken>()
        };
    }

    /// <summary>
    /// Aktualizuje czas ostatniego logowania na bieżący czas UTC
    /// </summary>
    public void UpdateLastLogin()
    {
        LastLoginAt = DateTimeOffset.UtcNow;
    }
}
