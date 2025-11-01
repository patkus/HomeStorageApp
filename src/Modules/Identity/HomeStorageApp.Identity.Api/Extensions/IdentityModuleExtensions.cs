using System.Text;
using HomeStorageApp.Identity.Core.Application.Commands.Login;
using HomeStorageApp.Identity.Core.Application.Commands.Logout;
using HomeStorageApp.Identity.Core.Application.Commands.Register;
using HomeStorageApp.Identity.Core.Application.Interfaces;
using HomeStorageApp.Identity.Core.Infrastructure.Security;
using HomeStorageApp.Identity.Persistence;
using HomeStorageApp.Identity.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace HomeStorageApp.Identity.Api.Extensions;

/// <summary>
/// Extension methods dla konfiguracji modułu Identity.
/// Rejestruje wszystkie usługi, repozytoria, handlery i konfigurację JWT.
/// </summary>
public static class IdentityModuleExtensions
{
    /// <summary>
    /// Dodaje wszystkie serwisy modułu Identity do DI container.
    /// Konfiguruje DbContext, repozytoria, handlery, security services i JWT authentication.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Application configuration</param>
    /// <returns>Service collection dla dalszego chainingu</returns>
    public static IServiceCollection AddIdentityModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Konfiguracja JWT Settings
        var jwtSettings = configuration
            .GetSection(JwtSettings.SectionName)
            .Get<JwtSettings>()
            ?? throw new InvalidOperationException("JWT settings are not configured in appsettings.json");

        services.AddSingleton(jwtSettings);

        // DbContext z PostgreSQL
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found");

        services.AddDbContext<IdentityDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Repozytoria
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        // Security Services
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddSingleton<ITokenGenerator, JwtTokenGenerator>();

        // Command Handlers
        services.AddScoped<RegisterCommandHandler>();
        services.AddScoped<LoginCommandHandler>();
        services.AddScoped<LogoutCommandHandler>();

        // JWT Authentication
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ClockSkew = TimeSpan.FromMinutes(5) // 5 minut tolerancji dla różnic czasu
                };
            });

        services.AddAuthorization();

        return services;
    }

    /// <summary>
    /// Aplikuje automatyczne migracje dla modułu Identity
    /// </summary>
    /// <param name="app">Aplikacja</param>
    public static async Task UseIdentityMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<IdentityDbContext>>();

        try
        {
            var context = services.GetRequiredService<IdentityDbContext>();
            await context.Database.MigrateAsync();
            logger.LogInformation("Identity database migration completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the Identity database");
            throw;
        }
    }
}
