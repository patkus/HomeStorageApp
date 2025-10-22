# API Endpoint Implementation Plan: Register, Login, Logout

## 1. Przegląd punktów końcowych

Ten plan obejmuje implementację trzech endpointów uwierzytelniania dla modułu Identity:

- **POST /api/auth/register** - Rejestracja nowego konta właściciela gospodarstwa domowego
- **POST /api/auth/login** - Uwierzytelnianie użytkownika i wydawanie tokenów JWT
- **POST /api/auth/logout** - Wylogowanie użytkownika poprzez unieważnienie tokenu

System wykorzystuje mechanizm JWT Bearer token z refresh tokenami dla bezpiecznej autentykacji. Każde gospodarstwo domowe ma jednego właściciela (Household Owner), który jest głównym kontem uwierzytelniającym. Profile członków gospodarstwa będą zarządzane jako podmioty wewnętrzne, ale nie będą miały własnych kont logowania.

## 2. Szczegóły żądań

### 2.1 POST /api/auth/register

**Metoda HTTP:** POST  
**Struktura URL:** `/api/auth/register`  
**Content-Type:** `application/json`

**Parametry:**
- **Wymagane:**
  - `email` (string) - Adres email użytkownika (format email, unikalny)
  - `password` (string) - Hasło użytkownika (min. 8 znaków, wymaga: 1 wielka, 1 mała, 1 cyfra, 1 znak specjalny)
  - `confirmPassword` (string) - Potwierdzenie hasła (musi być identyczne z password)

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "SecureP@ss123",
  "confirmPassword": "SecureP@ss123"
}
```

### 2.2 POST /api/auth/login

**Metoda HTTP:** POST  
**Struktura URL:** `/api/auth/login`  
**Content-Type:** `application/json`

**Parametry:**
- **Wymagane:**
  - `email` (string) - Adres email użytkownika
  - `password` (string) - Hasło użytkownika

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "SecureP@ss123"
}
```

### 2.3 POST /api/auth/logout

**Metoda HTTP:** POST  
**Struktura URL:** `/api/auth/logout`  
**Headers:** `Authorization: Bearer {access_token}`  
**Content-Type:** `application/json`

**Parametry:**
- **Wymagane:**
  - `refreshToken` (string) - Token odświeżania do unieważnienia

**Request Body:**
```json
{
  "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

## 3. Wykorzystywane typy

### 3.1 DTOs (Data Transfer Objects)

```csharp
// Request DTOs
public sealed record RegisterRequest(
    string Email,
    string Password,
    string ConfirmPassword);

public sealed record LoginRequest(
    string Email,
    string Password);

public sealed record LogoutRequest(string RefreshToken);

// Response DTOs
public sealed record AuthResponse(
    Guid UserId,
    string Email,
    string AccessToken,
    string RefreshToken,
    DateTimeOffset AccessTokenExpiresAt,
    DateTimeOffset RefreshTokenExpiresAt);

public sealed record LogoutResponse(
    bool Success,
    string Message);
```

### 3.2 Command Models (CQRS)

```csharp
// Commands
public sealed record RegisterCommand(
    string Email,
    string Password);

public sealed record LoginCommand(
    string Email,
    string Password);

public sealed record LogoutCommand(
    Guid UserId,
    string RefreshToken);

// Command Results
public sealed record RegisterResult(
    Guid UserId,
    string Email,
    string AccessToken,
    string RefreshToken,
    DateTimeOffset AccessTokenExpiresAt,
    DateTimeOffset RefreshTokenExpiresAt);

public sealed record LoginResult(
    Guid UserId,
    string Email,
    string AccessToken,
    string RefreshToken,
    DateTimeOffset AccessTokenExpiresAt,
    DateTimeOffset RefreshTokenExpiresAt);

public sealed record LogoutResult(bool Success);
```

### 3.3 Domain Entities

```csharp
public sealed class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? LastLoginAt { get; private set; }
    public bool IsActive { get; private set; }
    
    // Navigation property
    public ICollection<RefreshToken> RefreshTokens { get; private set; }
    
    private User() { } // EF Core
    
    public static User Create(string email, string passwordHash)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = passwordHash,
            CreatedAt = DateTimeOffset.UtcNow,
            IsActive = true,
            RefreshTokens = new List<RefreshToken>()
        };
    }
    
    public void UpdateLastLogin()
    {
        LastLoginAt = DateTimeOffset.UtcNow;
    }
}

public sealed class RefreshToken
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Token { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    
    // Navigation property
    public User User { get; private set; }
    
    private RefreshToken() { } // EF Core
    
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
    
    public void Revoke()
    {
        IsRevoked = true;
        RevokedAt = DateTimeOffset.UtcNow;
    }
    
    public bool IsValid() => !IsRevoked && ExpiresAt > DateTimeOffset.UtcNow;
}
```

### 3.4 Value Objects

```csharp
public sealed record Email
{
    public string Value { get; }
    
    private Email(string value) => Value = value;
    
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
            
        return Result<Email>.Success(new Email(value.ToLowerInvariant()));
    }
    
    private static bool IsValidEmailFormat(string email)
    {
        var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        return regex.IsMatch(email);
    }
}

public sealed record Password
{
    public string Value { get; }
    
    private Password(string value) => Value = value;
    
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
    
    private static bool HasUpperCase(string value) => value.Any(char.IsUpper);
    private static bool HasLowerCase(string value) => value.Any(char.IsLower);
    private static bool HasDigit(string value) => value.Any(char.IsDigit);
    private static bool HasSpecialChar(string value) => value.Any(c => !char.IsLetterOrDigit(c));
}
```

### 3.5 Result Type (dla obsługi błędów)

```csharp
public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T Value { get; }
    public string Error { get; }
    
    private Result(bool isSuccess, T value, string error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }
    
    public static Result<T> Success(T value) => new(true, value, string.Empty);
    public static Result<T> Failure(string error) => new(false, default!, error);
}
```

## 4. Szczegóły odpowiedzi

### 4.1 POST /api/auth/register

**Success Response (201 Created):**
```json
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "email": "user@example.com",
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "accessTokenExpiresAt": "2025-01-22T10:57:00Z",
  "refreshTokenExpiresAt": "2025-01-29T09:42:00Z"
}
```

**Error Responses:**

- **400 Bad Request** - Nieprawidłowe dane wejściowe
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Błąd walidacji",
  "status": 400,
  "errors": {
    "Email": ["Nieprawidłowy format email"],
    "Password": ["Hasło musi mieć minimum 8 znaków"],
    "ConfirmPassword": ["Hasła nie są zgodne"]
  }
}
```

- **409 Conflict** - Email już istnieje
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.10",
  "title": "Konflikt",
  "status": 409,
  "detail": "Użytkownik z podanym adresem email już istnieje"
}
```

### 4.2 POST /api/auth/login

**Success Response (200 OK):**
```json
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "email": "user@example.com",
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "accessTokenExpiresAt": "2025-01-22T10:57:00Z",
  "refreshTokenExpiresAt": "2025-01-29T09:42:00Z"
}
```

**Error Responses:**

- **400 Bad Request** - Nieprawidłowe dane wejściowe
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Błąd walidacji",
  "status": 400,
  "errors": {
    "Email": ["Email jest wymagany"],
    "Password": ["Hasło jest wymagane"]
  }
}
```

- **401 Unauthorized** - Nieprawidłowe dane logowania
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.2",
  "title": "Nieprawidłowe dane logowania",
  "status": 401,
  "detail": "Email lub hasło jest nieprawidłowe"
}
```

### 4.3 POST /api/auth/logout

**Success Response (200 OK):**
```json
{
  "success": true,
  "message": "Wylogowano pomyślnie"
}
```

**Error Responses:**

- **401 Unauthorized** - Brak lub nieprawidłowy token
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.2",
  "title": "Nieautoryzowany",
  "status": 401,
  "detail": "Token jest nieprawidłowy lub wygasł"
}
```

- **400 Bad Request** - Nieprawidłowy refresh token
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Błąd walidacji",
  "status": 400,
  "detail": "Refresh token jest nieprawidłowy lub wygasł"
}
```

## 5. Przepływ danych

### 5.1 Register Flow

```
Client Request
    ↓
[API Endpoint] POST /api/auth/register
    ↓
[Validation] Walidacja RegisterRequest
    ↓
[Mapping] RegisterRequest → RegisterCommand
    ↓
[Command Handler] RegisterCommandHandler
    ↓
[Domain Validation] 
    - Email format & uniqueness
    - Password complexity
    - Password confirmation match
    ↓
[Password Hashing] PBKDF2 (100k iterations, SHA256)
    ↓
[Entity Creation] User.Create()
    ↓
[Persistence] UserRepository.AddAsync()
    ↓
[Token Generation]
    - JWT Access Token (15 min)
    - Refresh Token (7 days)
    ↓
[Persistence] RefreshTokenRepository.AddAsync()
    ↓
[Unit of Work] SaveChangesAsync()
    ↓
[Mapping] RegisterResult → AuthResponse
    ↓
Client Response (201 Created)
```

### 5.2 Login Flow

```
Client Request
    ↓
[API Endpoint] POST /api/auth/login
    ↓
[Validation] Walidacja LoginRequest
    ↓
[Mapping] LoginRequest → LoginCommand
    ↓
[Command Handler] LoginCommandHandler
    ↓
[User Lookup] UserRepository.GetByEmailAsync()
    ↓
[Password Verification] PBKDF2 timing-safe comparison
    ↓
[Check Active Status] User.IsActive
    ↓
[Update Last Login] User.UpdateLastLogin()
    ↓
[Token Generation]
    - JWT Access Token (15 min)
    - Refresh Token (7 days)
    ↓
[Revoke Old Tokens] RefreshTokenRepository.RevokeAllForUser()
    ↓
[Persistence] RefreshTokenRepository.AddAsync()
    ↓
[Unit of Work] SaveChangesAsync()
    ↓
[Mapping] LoginResult → AuthResponse
    ↓
Client Response (200 OK)
```

### 5.3 Logout Flow

```
Client Request (with Bearer Token)
    ↓
[API Endpoint] POST /api/auth/logout
    ↓
[Authentication] JWT Middleware validates token
    ↓
[Extract User ID] From JWT claims
    ↓
[Validation] Walidacja LogoutRequest
    ↓
[Mapping] LogoutRequest → LogoutCommand
    ↓
[Command Handler] LogoutCommandHandler
    ↓
[Token Lookup] RefreshTokenRepository.GetByTokenAsync()
    ↓
[Validation] Verify token belongs to user
    ↓
[Revoke Token] RefreshToken.Revoke()
    ↓
[Persistence] RefreshTokenRepository.UpdateAsync()
    ↓
[Unit of Work] SaveChangesAsync()
    ↓
[Optional] Add access token to blacklist cache
    ↓
[Mapping] LogoutResult → LogoutResponse
    ↓
Client Response (200 OK)
```

## 6. Względy bezpieczeństwa

### 6.1 Hashowanie haseł

- **Algorytm:** PBKDF2 (Password-Based Key Derivation Function 2)
- **Biblioteka:** `System.Security.Cryptography` (wbudowana w .NET)
- **Parametry:** 100,000 iteracji, SHA256, 256-bit salt, 256-bit hash
- **Implementacja:**
```csharp
public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}

public sealed class Pbkdf2PasswordHasher : IPasswordHasher
{
    private const int Iterations = 100_000;
    private const int SaltSize = 32; // 256 bits
    private const int HashSize = 32; // 256 bits
    
    public string HashPassword(string password)
    {
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[SaltSize];
        rng.GetBytes(salt);
        
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize
        );
        
        // Format: iterations.salt.hash (all base64 encoded)
        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }
    
    public bool VerifyPassword(string password, string passwordHash)
    {
        var parts = passwordHash.Split('.');
        if (parts.Length != 3)
        {
            return false;
        }
        
        var iterations = int.Parse(parts[0]);
        var salt = Convert.FromBase64String(parts[1]);
        var hash = Convert.FromBase64String(parts[2]);
        
        var hashToVerify = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            hash.Length
        );
        
        return CryptographicOperations.FixedTimeEquals(hash, hashToVerify);
    }
}
```

**Zalety PBKDF2:**
- Wbudowany w .NET (System.Security.Cryptography)
- Rekomendowany przez OWASP i NIST
- Odporny na ataki brute-force dzięki wysokiej liczbie iteracji
- Używa bezpiecznego porównania w stałym czasie (timing-safe)

### 6.2 JWT Configuration

```csharp
public sealed class JwtSettings
{
    public required string Secret { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required int AccessTokenExpirationMinutes { get; init; } // 15
    public required int RefreshTokenExpirationDays { get; init; } // 7
}

public interface ITokenGenerator
{
    string GenerateAccessToken(Guid userId, string email);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateToken(string token);
}

public sealed class JwtTokenGenerator : ITokenGenerator
{
    private readonly JwtSettings _settings;
    
    public string GenerateAccessToken(Guid userId, string email)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTimeOffset.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes).DateTime,
            signingCredentials: credentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}
```

### 6.3 Rate Limiting

Implementacja rate limiting dla endpointów auth:
- Register: 5 prób/godzinę na IP
- Login: 5 prób/5 minut na IP
- Logout: 10 prób/minutę na użytkownika

### 6.4 HTTPS Only

- Wszystkie endpointy auth wymagają HTTPS
- Konfiguracja HSTS (HTTP Strict Transport Security)
- Secure i HttpOnly cookies dla refresh tokenów (opcjonalnie)

### 6.5 Input Sanitization

- Walidacja wszystkich wejść przed przetwarzaniem
- Escape SQL injection poprzez Entity Framework parametryzowane zapytania
- XSS protection poprzez walidację i enkodowanie

### 6.6 Token Blacklisting (opcjonalne)

Dla access tokenów przed wygaśnięciem:
```csharp
public interface ITokenBlacklist
{
    Task AddToBlacklistAsync(string token, TimeSpan expiresIn);
    Task<bool> IsBlacklistedAsync(string token);
}

// Implementacja z Redis lub In-Memory Cache
public sealed class RedisTokenBlacklist : ITokenBlacklist
{
    private readonly IDistributedCache _cache;
    
    public async Task AddToBlacklistAsync(string token, TimeSpan expiresIn)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiresIn
        };
        await _cache.SetStringAsync($"blacklist:{token}", "1", options);
    }
    
    public async Task<bool> IsBlacklistedAsync(string token)
    {
        var value = await _cache.GetStringAsync($"blacklist:{token}");
        return value != null;
    }
}
```

## 7. Obsługa błędów

### 7.1 Hierarchia wyjątków domenowych

```csharp
public abstract class DomainException(string message) : Exception(message);

public sealed record UserAlreadyExistsException(string Email) 
    : DomainException($"Użytkownik z adresem email '{Email}' już istnieje");

public sealed record InvalidCredentialsException() 
    : DomainException("Email lub hasło jest nieprawidłowe");

public sealed record InvalidTokenException() 
    : DomainException("Token jest nieprawidłowy lub wygasł");

public sealed record UserNotActiveException() 
    : DomainException("Konto użytkownika jest nieaktywne");
```

### 7.2 Global Exception Handler

```csharp
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = exception switch
        {
            UserAlreadyExistsException e => (409, "Konflikt", e.Message),
            InvalidCredentialsException e => (401, "Nieprawidłowe dane logowania", e.Message),
            InvalidTokenException e => (401, "Nieautoryzowany", e.Message),
            UserNotActiveException e => (403, "Dostęp zabroniony", e.Message),
            ValidationException e => (400, "Błąd walidacji", e.Message),
            _ => (500, "Błąd serwera", "Wystąpił nieoczekiwany błąd")
        };
        
        _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);
        
        var problemDetails = new ProblemDetails
        {
            Type = $"https://tools.ietf.org/html/rfc9110#section-15.5.{statusCode}",
            Title = title,
            Status = statusCode,
            Detail = detail
        };
        
        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        
        return true;
    }
}
```

### 7.3 Scenariusze błędów

| Endpoint | Scenariusz | Status Code | Wyjątek/Komunikat |
|----------|-----------|-------------|-------------------|
| Register | Email już istnieje | 409 | UserAlreadyExistsException |
| Register | Nieprawidłowy format email | 400 | ValidationException |
| Register | Słabe hasło | 400 | ValidationException |
| Register | Hasła nie są zgodne | 400 | ValidationException |
| Login | Nieprawidłowy email/hasło | 401 | InvalidCredentialsException |
| Login | Konto nieaktywne | 403 | UserNotActiveException |
| Login | Brak wymaganych pól | 400 | ValidationException |
| Logout | Nieprawidłowy token | 401 | InvalidTokenException |
| Logout | Token wygasł | 401 | InvalidTokenException |
| Logout | Brak autoryzacji | 401 | UnauthorizedException |
| All | Błąd serwera | 500 | Internal Server Error |

## 8. Rozważania dotyczące wydajności

### 8.1 Database Indexes

```sql
-- Users table
CREATE UNIQUE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Users_IsActive ON Users(IsActive);

-- RefreshTokens table
CREATE INDEX IX_RefreshTokens_UserId ON RefreshTokens(UserId);
CREATE UNIQUE INDEX IX_RefreshTokens_Token ON RefreshTokens(Token);
CREATE INDEX IX_RefreshTokens_ExpiresAt ON RefreshTokens(ExpiresAt) WHERE IsRevoked = false;
```

### 8.2 Caching Strategy

- **User by Email:** Cache na 5 minut (dla częstych loginów)
- **Refresh Tokens:** Bez cache (zawsze weryfikuj z DB dla bezpieczeństwa)
- **Token Blacklist:** Redis z automatycznym wygasaniem

### 8.3 Connection Pooling

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=homestorage;Username=postgres;Password=***;Pooling=true;Minimum Pool Size=5;Maximum Pool Size=100"
  }
}
```

### 8.4 Async/Await

Wszystkie operacje I/O (database, cache) używają async/await dla lepszej skalowalności.

### 8.5 Query Optimization

```csharp
// Eager loading dla refresh tokens
public async Task<User?> GetByEmailWithTokensAsync(string email)
{
    return await _context.Users
        .Include(u => u.RefreshTokens.Where(rt => !rt.IsRevoked))
        .FirstOrDefaultAsync(u => u.Email == email);
}

// Projection dla lightweight queries
public async Task<bool> EmailExistsAsync(string email)
{
    return await _context.Users
        .AnyAsync(u => u.Email == email);
}
```

## 9. Etapy wdrożenia

### Faza 1: Infrastruktura i konfiguracja (Core)

1. **Dodanie zależności NuGet do Identity.Core:**
   ```xml
   <PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="7.0.3" />
   ```
   
   **Uwaga:** System.Security.Cryptography (dla PBKDF2) jest wbudowany w .NET, nie wymaga dodatkowych pakietów.

2. **Utworzenie struktury folderów w Identity.Core (nazwy biznesowe):**
   ```
   Domain/
     Users/
       User.cs
     Authentication/
       RefreshToken.cs
     Email/
       Email.cs
     Password/
       Password.cs
     Errors/
       DomainException.cs
       UserAlreadyExistsException.cs
       InvalidCredentialsException.cs
       InvalidTokenException.cs
       UserNotActiveException.cs
   Common/
     Result.cs
   ```

3. **Implementacja domenowych encji:**
   - User entity z metodami fabrycznymi
   - RefreshToken entity z logiką walidacji
   - Value objects (Email, Password)

4. **Implementacja Result type** dla functional error handling

### Faza 2: Application Layer (Core)

5. **Utworzenie struktury CQRS:**
   ```
   Application/
     Commands/
       Register/
         RegisterCommand.cs
         RegisterCommandHandler.cs
         RegisterCommandValidator.cs
       Login/
         LoginCommand.cs
         LoginCommandHandler.cs
         LoginCommandValidator.cs
       Logout/
         LogoutCommand.cs
         LogoutCommandHandler.cs
         LogoutCommandValidator.cs
     DTOs/
       AuthResponse.cs
       LogoutResponse.cs
     Interfaces/
       IPasswordHasher.cs
       ITokenGenerator.cs
       ITokenBlacklist.cs
       IUserRepository.cs
       IRefreshTokenRepository.cs
   ```

6. **Implementacja command handlers** z logiką biznesową

7. **Implementacja walidatorów** dla każdego command

8. **Utworzenie interfejsów** dla abstrakcji infrastruktury

### Faza 3: Infrastructure (Persistence)

9. **Dodanie zależności NuGet do Identity.Persistence:**
   ```xml
   <PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.0" />
   <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.0.0" />
   ```

10. **Utworzenie Identity DbContext:**
    ```csharp
    public sealed class IdentityDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
    ```

11. **Implementacja Entity Configurations:**
    - UserConfiguration (indexes, constraints)
    - RefreshTokenConfiguration (indexes, relationships)

12. **Implementacja repozytoriów:**
    - UserRepository (GetByEmailAsync, EmailExistsAsync, AddAsync)
    - RefreshTokenRepository (GetByTokenAsync, RevokeAllForUser, AddAsync, UpdateAsync)

13. **Implementacja Unit of Work pattern** dla transakcji

### Faza 4: Security Infrastructure (Core lub nowa warstwa Shared)

14. **Implementacja IPasswordHasher** z PBKDF2

15. **Implementacja ITokenGenerator** z JWT

16. **Konfiguracja JWT settings** w appsettings.json:
    ```json
    {
      "Jwt": {
        "Secret": "your-256-bit-secret-key-here-min-32-chars",
        "Issuer": "HomeStorageApp",
        "Audience": "HomeStorageApp",
        "AccessTokenExpirationMinutes": 15,
        "RefreshTokenExpirationDays": 7
      }
    }
    ```

### Faza 5: API Layer (Api)

17. **Dodanie zależności NuGet do Identity.Api:**
    ```xml
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="9.0.0" />
    ```

18. **Utworzenie Minimal API endpoints:**
    ```
    Endpoints/
      AuthEndpoints.cs
    ```

19. **Implementacja Minimal API endpoints:**
    - Register endpoint z walidacją
    - Login endpoint z walidacją
    - Logout endpoint (wymagające autoryzacji)

20. **Konfiguracja Extension Methods** dla DI:
    ```csharp
    public static class IdentityModuleExtensions
    {
        public static IServiceCollection AddIdentityModule(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // DbContext registration
            // Repository registration
            // Service registration
            // JWT Authentication configuration
            return services;
        }
        
        public static IEndpointRouteBuilder MapIdentityEndpoints(
            this IEndpointRouteBuilder endpoints)
        {
            // Map auth endpoints
            return endpoints;
        }
    }
    ```

### Faza 6: Integracja z główną aplikacją

21. **Aktualizacja HomeStorageApp.csproj:**
    ```xml
    <ItemGroup>
      <ProjectReference Include="..\src\Modules\Identity\HomeStorageApp.Identity.Api\HomeStorageApp.Identity.Api.csproj" />
    </ItemGroup>
    ```

22. **Konfiguracja w Program.cs:**
    ```csharp
    var builder = WebApplication.CreateBuilder(args);
    
    // Add Identity module
    builder.Services.AddIdentityModule(builder.Configuration);
    
    // Add global exception handler
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();
    
    var app = builder.Build();
    
    // Configure middleware pipeline
    app.UseExceptionHandler();
    app.UseAuthentication();
    app.UseAuthorization();
    
    // Map Identity endpoints
    app.MapIdentityEndpoints();
    
    app.Run();
    ```

23. **Konfiguracja appsettings.json:**
    - Connection string dla PostgreSQL
    - JWT settings
    - Logging configuration

### Faza 7: Migracje bazy danych

24. **Utworzenie migracji EF Core:**
    ```bash
    dotnet ef migrations add InitialIdentitySchema -p src/Modules/Identity/HomeStorageApp.Identity.Persistence -s HomeStorageApp
    ```

25. **Aktualizacja bazy danych:**
    ```bash
    dotnet ef database update -p src/Modules/Identity/HomeStorageApp.Identity.Persistence -s HomeStorageApp
    ```

26. **Weryfikacja schemy bazy danych:**
    - Tabela Users z odpowiednimi kolumnami i indexami
    - Tabela RefreshTokens z relacją do Users
    - Wszystkie constrainty i indexy utworzone poprawnie

### Faza 8: Testowanie

27. **Testowanie endpoint register:**
    - Pozytywny scenariusz (nowy użytkownik)
    - Email już istnieje (409)
    - Nieprawidłowy format email (400)
    - Słabe hasło (400)
    - Hasła nie pasują (400)

28. **Testowanie endpoint login:**
    - Pozytywny scenariusz (poprawne dane)
    - Nieprawidłowy email/hasło (401)
    - Nieaktywne konto (403)
    - Weryfikacja generowanych tokenów

29. **Testowanie endpoint logout:**
    - Pozytywny scenariusz (unieważnienie tokenu)
    - Bez autoryzacji (401)
    - Nieprawidłowy refresh token (400)
    - Token już unieważniony

30. **Testowanie bezpieczeństwa:**
    - Weryfikacja hashowania haseł (nie można odczytać z DB)
    - Weryfikacja JWT tokenów (signature, expiration)
    - Test rate limiting (jeśli zaimplementowane)

### Faza 9: Dokumentacja i finalizacja

31. **Dodanie dokumentacji API** do Swagger/OpenAPI:
    - Opisy endpointów
    - Przykłady request/response
    - Kody statusu i obsługa błędów

32. **Przygotowanie pliku README** dla modułu Identity:
    - Opis architektury
    - Instrukcje konfiguracji
    - Przykłady użycia

33. **Przegląd kodu i refaktoring:**
    - Code review
    - Usunięcie nieużywanego kodu
    - Optymalizacja wydajności

34. **Przygotowanie do deploymentu:**
    - Weryfikacja connection strings
    - Konfiguracja environment variables
    - Docker configuration (jeśli potrzebne)

## 10. Uwagi końcowe

### Zalecenia implementacyjne

1. **Separation of Concerns:** Ściśle przestrzegaj podziału na warstwy (Domain, Application, Infrastructure, API)
2. **Immutability:** Używaj `record` i `readonly` gdzie to możliwe
3. **Null Safety:** Wykorzystuj nullable reference types (.NET 9)
4. **Async/Await:** Wszystkie operacje I/O muszą być asynchroniczne
5. **Error Handling:** Wykorzystuj Result pattern zamiast exceptions dla flow control
6. **Security First:** Nigdy nie loguj wrażliwych danych (hasła, tokeny)

### Potencjalne rozszerzenia

1. **Email Verification:** Dodanie weryfikacji email przed aktywacją konta
2. **Password Reset:** Funkcjonalność resetowania hasła przez email
3. **Two-Factor Authentication (2FA):** Dodatkowa warstwa bezpieczeństwa
4. **Account Lockout:** Blokada konta po wielu nieudanych próbach logowania
5. **Refresh Token Rotation:** Automatyczna rotacja refresh tokenów dla lepszego bezpieczeństwa
6. **OAuth2/OpenID Connect:** Integracja z zewnętrznymi providerami (Google, Facebook)
7. **Audit Log:** Logowanie wszystkich operacji uwierzytelniania
8. **Session Management:** Zarządzanie aktywnymi sesjami użytkownika

### Checklist przed produkcją

- [ ] Wszystkie hasła są hashowane z PBKDF2 (100k iteracji)
- [ ] JWT secret jest silny (min. 256 bitów) i przechowywany bezpiecznie
- [ ] HTTPS jest wymuszane na wszystkich endpointach auth
- [ ] Rate limiting jest skonfigurowane i przetestowane
- [ ] Wszystkie błędy są obsługiwane i logowane
- [ ] Database indexy są utworzone dla optymalizacji
- [ ] Migracje są przetestowane na staging environment
- [ ] Dokumentacja API jest aktualna
- [ ] Testy integracyjne przechodzą
- [ ] Security audit został przeprowadzony
