using HomeStorageApp.Shared.Exceptions;

namespace HomeStorageApp.Identity.Core.Domain.Errors;

/// <summary>
/// Wyjątek rzucany, gdy token JWT lub refresh token jest nieprawidłowy lub wygasł.
/// Zwracany jako odpowiedź 401 Unauthorized w API.
/// </summary>
public sealed class InvalidTokenException() 
    : UnauthorizedException("Token jest nieprawidłowy lub wygasł");
