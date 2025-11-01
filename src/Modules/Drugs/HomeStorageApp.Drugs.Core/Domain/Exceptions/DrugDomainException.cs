using HomeStorageApp.Shared;

namespace HomeStorageApp.Drugs.Core.Domain.Exceptions;

/// <summary>
/// Bazowy wyjątek domenowy dla modułu Drugs
/// </summary>
public class DrugDomainException(string message) : DomainException(message);
