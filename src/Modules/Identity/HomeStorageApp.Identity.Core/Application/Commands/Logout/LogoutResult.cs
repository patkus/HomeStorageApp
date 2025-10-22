namespace HomeStorageApp.Identity.Core.Application.Commands.Logout;

/// <summary>
/// Result zwracany przez LogoutCommandHandler po pomyślnym wylogowaniu.
/// Zawiera informację o powodzeniu operacji.
/// </summary>
/// <param name="Success">Określa, czy operacja się powiodła (zawsze true po poprawnym wykonaniu)</param>
public sealed record LogoutResult(bool Success);
