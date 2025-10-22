namespace HomeStorageApp.Identity.Core.Application.Commands.Register;

/// <summary>
/// Command reprezentujący żądanie rejestracji nowego użytkownika.
/// Część wzorca CQRS - oddziela request API od logiki biznesowej.
/// </summary>
/// <param name="Email">Adres email nowego użytkownika</param>
/// <param name="Password">Hasło użytkownika (zostanie zahashowane przed zapisem)</param>
public sealed record RegisterCommand(
    string Email,
    string Password);
