namespace HomeStorageApp.Drugs.Core.Domain.SystemUnits;

/// <summary>
/// Kategorie systemowych jednostek miar
/// </summary>
public enum SystemUnitCategory
{
    /// <summary>
    /// Jednostki sztukowe (tabletka, kapsułka, sztuka)
    /// </summary>
    Piece,
    
    /// <summary>
    /// Jednostki objętości (ml, l)
    /// </summary>
    Volume,
    
    /// <summary>
    /// Jednostki masy (g, kg)
    /// </summary>
    Weight,
    
    /// <summary>
    /// Inne jednostki (saszetka, etc.)
    /// </summary>
    Other
}
