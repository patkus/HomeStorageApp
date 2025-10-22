namespace HomeStorageApp.Identity.Core.Application.Commands.Login;

/// <summary>
/// Command reprezentujący żądanie logowania użytkownika.
/// Część wzorca CQRS - oddziela request API od logiki biznesowej.
/// </summary>
/// <param name="Email">Adres email użytkownika</param>
/// <param name="Password">Hasło użytkownika w formie plaintext</param>
public sealed record LoginCommand(
    string Email,
    string Password);
