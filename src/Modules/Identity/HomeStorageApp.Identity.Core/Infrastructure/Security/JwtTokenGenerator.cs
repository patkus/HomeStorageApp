using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using HomeStorageApp.Identity.Core.Application.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace HomeStorageApp.Identity.Core.Infrastructure.Security;

/// <summary>
/// Implementacja generowania tokenów JWT dla uwierzytelniania.
/// Generuje access tokeny (JWT) i refresh tokeny (random bytes).
/// </summary>
public sealed class JwtTokenGenerator(JwtSettings settings) : ITokenGenerator
{
    /// <summary>
    /// Generuje JWT access token zawierający claims użytkownika.
    /// Token jest podpisany kluczem Secret i wygasa po czasie określonym w konfiguracji.
    /// </summary>
    /// <param name="userId">Unikalny identyfikator użytkownika</param>
    /// <param name="email">Adres email użytkownika</param>
    /// <returns>Zakodowany JWT token jako string</returns>
    public string GenerateAccessToken(Guid userId, string email)
    {
        // Definiuj claims (dane w tokenie)
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()), // Subject (user ID)
            new Claim(JwtRegisteredClaimNames.Email, email), // Email
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID (unique token ID)
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()) // Standard ASP.NET claim
        };

        // Utwórz klucz symetryczny z Secret
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Utwórz token JWT
        var token = new JwtSecurityToken(
            issuer: settings.Issuer,
            audience: settings.Audience,
            claims: claims,
            expires: DateTimeOffset.UtcNow.AddMinutes(settings.AccessTokenExpirationMinutes).DateTime,
            signingCredentials: credentials
        );

        // Zakoduj token do string
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Generuje bezpieczny refresh token używając kryptograficznie silnego generatora losowego.
    /// Token składa się z 64 losowych bajtów zakodowanych w base64.
    /// </summary>
    /// <returns>Refresh token jako base64 string</returns>
    public string GenerateRefreshToken()
    {
        // Generuj 64 losowe bajty (512 bits)
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        // Konwertuj na base64 string
        return Convert.ToBase64String(randomBytes);
    }
}
