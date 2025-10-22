namespace HomeStorageApp.Shared;

/// <summary>
/// Bazowa klasa dla wszystkich wyjątków domenowych w aplikacji.
/// Używana do odróżnienia błędów biznesowych od błędów technicznych.
/// Każdy moduł może definiować własne konkretne wyjątki dziedziczące po tej klasie.
/// </summary>
/// <param name="message">Komunikat opisujący błąd domenowy</param>
public abstract class DomainException(string message) : Exception(message);
