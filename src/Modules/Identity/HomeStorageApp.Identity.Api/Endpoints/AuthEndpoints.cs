using HomeStorageApp.Identity.Core.Application.Commands.Login;
using HomeStorageApp.Identity.Core.Application.Commands.Logout;
using HomeStorageApp.Identity.Core.Application.Commands.Register;
using HomeStorageApp.Identity.Core.Application.DTOs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace HomeStorageApp.Identity.Api.Endpoints;

/// <summary>
/// Definiuje endpointy API dla uwierzytelniania (rejestracja, logowanie, wylogowanie).
/// Używa Minimal API pattern z ASP.NET Core.
/// </summary>
public static class AuthEndpoints
{
    /// <summary>
    /// Mapuje wszystkie endpointy związane z uwierzytelnianiem do route buildera.
    /// </summary>
    /// <param name="app">Route builder do mapowania endpointów</param>
    /// <returns>Route builder dla dalszego chainingu</returns>
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Authentication");

        // POST /api/auth/register
        group.MapPost("/register", RegisterAsync)
            .WithName("Register")
            .WithSummary("Rejestruje nowego użytkownika")
            .WithDescription("Tworzy nowe konto właściciela gospodarstwa domowego z emailem i hasłem")
            .Produces<AuthResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status409Conflict);

        // POST /api/auth/login
        group.MapPost("/login", LoginAsync)
            .WithName("Login")
            .WithSummary("Loguje użytkownika")
            .WithDescription("Uwierzytelnia użytkownika i zwraca tokeny JWT")
            .Produces<AuthResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized);

        // POST /api/auth/logout
        group.MapPost("/logout", LogoutAsync)
            .WithName("Logout")
            .WithSummary("Wylogowuje użytkownika")
            .WithDescription("Unieważnia refresh token użytkownika")
            .RequireAuthorization() // Wymaga JWT token
            .Produces<LogoutResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem();

        return app;
    }

    /// <summary>
    /// Endpoint rejestracji nowego użytkownika.
    /// POST /api/auth/register
    /// </summary>
    private static async Task<IResult> RegisterAsync(
        [FromBody] RegisterRequest request,
        RegisterCommandHandler handler,
        CancellationToken cancellationToken)
    {
        // Walidacja potwierdzenia hasła
        if (request.Password != request.ConfirmPassword)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                { "ConfirmPassword", new[] { "Hasła nie są zgodne" } }
            });
        }

        // Mapowanie Request -> Command
        var command = new RegisterCommand(request.Email, request.Password);

        // Wykonanie Command przez Handler
        var result = await handler.HandleAsync(command, cancellationToken);

        // Mapowanie Result -> Response
        var response = new AuthResponse(
            result.UserId,
            result.Email,
            result.AccessToken,
            result.RefreshToken,
            result.AccessTokenExpiresAt,
            result.RefreshTokenExpiresAt);

        return Results.Created($"/api/users/{result.UserId}", response);
    }

    /// <summary>
    /// Endpoint logowania użytkownika.
    /// POST /api/auth/login
    /// </summary>
    private static async Task<IResult> LoginAsync(
        [FromBody] LoginRequest request,
        LoginCommandHandler handler,
        CancellationToken cancellationToken)
    {
        // Mapowanie Request -> Command
        var command = new LoginCommand(request.Email, request.Password);

        // Wykonanie Command przez Handler
        var result = await handler.HandleAsync(command, cancellationToken);

        // Mapowanie Result -> Response
        var response = new AuthResponse(
            result.UserId,
            result.Email,
            result.AccessToken,
            result.RefreshToken,
            result.AccessTokenExpiresAt,
            result.RefreshTokenExpiresAt);

        return Results.Ok(response);
    }

    /// <summary>
    /// Endpoint wylogowania użytkownika.
    /// POST /api/auth/logout
    /// Wymaga autoryzacji (JWT Bearer token).
    /// </summary>
    private static async Task<IResult> LogoutAsync(
        [FromBody] LogoutRequest request,
        HttpContext httpContext,
        LogoutCommandHandler handler,
        CancellationToken cancellationToken)
    {
        // Pobierz userId z JWT claims
        var userIdClaim = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Results.Unauthorized();
        }

        // Mapowanie Request -> Command
        var command = new LogoutCommand(userId, request.RefreshToken);

        // Wykonanie Command przez Handler
        var result = await handler.HandleAsync(command, cancellationToken);

        // Mapowanie Result -> Response
        var response = new LogoutResponse(
            result.Success,
            "Wylogowano pomyślnie");

        return Results.Ok(response);
    }
}
