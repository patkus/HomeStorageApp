using HomeStorageApp.Shared.Exceptions;

namespace HomeStorageApp.Drugs.Core.Domain.Exceptions;

/// <summary>
/// Wyjątek rzucany gdy lek nie został znaleziony
/// </summary>
public sealed class DrugNotFoundException(string message) : NotFoundException(message);
