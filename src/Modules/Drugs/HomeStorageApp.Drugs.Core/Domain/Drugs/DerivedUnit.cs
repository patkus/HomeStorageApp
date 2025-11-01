namespace HomeStorageApp.Drugs.Core.Domain.Drugs;

/// <summary>
/// Encja reprezentująca jednostkę pochodną specyficzną dla leku
/// </summary>
public sealed class DerivedUnit
{
    /// <summary>
    /// Unikalny identyfikator jednostki pochodnej
    /// </summary>
    public Guid Id { get; private set; }
    
    /// <summary>
    /// Identyfikator leku, do którego należy jednostka
    /// </summary>
    public Guid DrugId { get; private set; }
    
    /// <summary>
    /// Nazwa jednostki pochodnej (np. "blister", "opakowanie")
    /// </summary>
    public string Name { get; private set; } = string.Empty;
    
    /// <summary>
    /// Identyfikator jednostki bazowej (może być PrimaryUnit lub inna DerivedUnit)
    /// </summary>
    public Guid BaseUnitId { get; private set; }
    
    /// <summary>
    /// Przelicznik do jednostki bazowej
    /// </summary>
    public decimal ConversionFactor { get; private set; }
    
    /// <summary>
    /// Czy to domyślna jednostka zakupu (używana przy skanowaniu)
    /// </summary>
    public bool IsDefaultPurchaseUnit { get; private set; }
    
    private readonly List<string> _barcodes = new();
    
    /// <summary>
    /// Lista kodów kreskowych przypisanych do tej jednostki
    /// </summary>
    public IReadOnlyList<string> Barcodes => _barcodes.AsReadOnly();
    
    /// <summary>
    /// Data utworzenia jednostki
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; }
    
    /// <summary>
    /// Lek, do którego należy jednostka (navigation property)
    /// </summary>
    public Drug Drug { get; private set; } = null!;
    
    /// <summary>
    /// Konstruktor bezparametrowy dla EF Core
    /// </summary>
    private DerivedUnit()
    {
    }
    
    /// <summary>
    /// Tworzy nową jednostkę pochodną
    /// </summary>
    /// <param name="drugId">Identyfikator leku</param>
    /// <param name="name">Nazwa jednostki</param>
    /// <param name="baseUnitId">Identyfikator jednostki bazowej</param>
    /// <param name="conversionFactor">Przelicznik do jednostki bazowej</param>
    /// <param name="isDefault">Czy to domyślna jednostka zakupu</param>
    /// <param name="barcodes">Lista kodów kreskowych</param>
    /// <returns>Nowa instancja DerivedUnit</returns>
    public static DerivedUnit Create(
        Guid drugId, 
        string name, 
        Guid baseUnitId, 
        decimal conversionFactor, 
        bool isDefault,
        List<string>? barcodes)
    {
        var unit = new DerivedUnit
        {
            Id = Guid.NewGuid(),
            DrugId = drugId,
            Name = name,
            BaseUnitId = baseUnitId,
            ConversionFactor = conversionFactor,
            IsDefaultPurchaseUnit = isDefault,
            CreatedAt = DateTimeOffset.UtcNow
        };
        
        if (barcodes is not null)
        {
            unit._barcodes.AddRange(barcodes);
        }
        
        return unit;
    }
    
    /// <summary>
    /// Aktualizuje jednostkę pochodną
    /// </summary>
    /// <param name="name">Nowa nazwa jednostki</param>
    /// <param name="conversionFactor">Nowy przelicznik</param>
    /// <param name="isDefault">Czy to domyślna jednostka zakupu</param>
    /// <param name="barcodes">Nowa lista kodów kreskowych</param>
    public void Update(string name, decimal conversionFactor, bool isDefault, List<string>? barcodes)
    {
        Name = name;
        ConversionFactor = conversionFactor;
        IsDefaultPurchaseUnit = isDefault;
        
        _barcodes.Clear();
        if (barcodes is not null)
        {
            _barcodes.AddRange(barcodes);
        }
    }
}
