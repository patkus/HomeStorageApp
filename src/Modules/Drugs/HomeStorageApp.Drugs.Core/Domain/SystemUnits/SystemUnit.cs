namespace HomeStorageApp.Drugs.Core.Domain.SystemUnits;

/// <summary>
/// Encja reprezentująca systemową jednostkę miary
/// </summary>
public sealed class SystemUnit
{
    /// <summary>
    /// Unikalny identyfikator jednostki systemowej
    /// </summary>
    public Guid Id { get; private set; }
    
    /// <summary>
    /// Nazwa jednostki (np. "tabletka", "mililitr")
    /// </summary>
    public string Name { get; private set; } = string.Empty;
    
    /// <summary>
    /// Symbol jednostki (np. "tab", "ml")
    /// </summary>
    public string Symbol { get; private set; } = string.Empty;
    
    /// <summary>
    /// Kategoria jednostki
    /// </summary>
    public SystemUnitCategory Category { get; private set; }
    
    /// <summary>
    /// Konstruktor bezparametrowy dla EF Core
    /// </summary>
    private SystemUnit()
    {
    }
    
    /// <summary>
    /// Tworzy nową systemową jednostkę miary
    /// </summary>
    /// <param name="id">Unikalny identyfikator</param>
    /// <param name="name">Nazwa jednostki</param>
    /// <param name="symbol">Symbol jednostki</param>
    /// <param name="category">Kategoria jednostki</param>
    /// <returns>Nowa instancja SystemUnit</returns>
    public static SystemUnit Create(Guid id, string name, string symbol, SystemUnitCategory category)
    {
        return new SystemUnit
        {
            Id = id,
            Name = name,
            Symbol = symbol,
            Category = category
        };
    }
}
