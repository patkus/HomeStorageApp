namespace HomeStorageApp.Identity.Core.Application.Commands.Logout;

/// <summary>
/// Command reprezentujący żądanie wylogowania użytkownika.
/// Część wzorca CQRS - oddziela request API od logiki biznesowej.
/// </summary>
/// <param name="UserId">Identyfikator użytkownika (pobrany z JWT claims)</param>
/// <param name="RefreshToken">Refresh token do unieważnienia</param>
public sealed record LogoutCommand(
    Guid UserId,
    string RefreshToken);
