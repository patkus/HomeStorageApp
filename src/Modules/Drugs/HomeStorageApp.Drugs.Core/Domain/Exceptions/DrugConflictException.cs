using HomeStorageApp.Shared.Exceptions;

namespace HomeStorageApp.Drugs.Core.Domain.Exceptions;

/// <summary>
/// Wyjątek rzucany gdy występuje konflikt (np. duplikat kodu kreskowego)
/// </summary>
public sealed class DrugConflictException(string message) : ConflictException(message);
