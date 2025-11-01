using HomeStorageApp.Drugs.Core.Domain.SystemUnits;

namespace HomeStorageApp.Drugs.Core.Domain.Drugs;

/// <summary>
/// Encja reprezentująca lek w systemie
/// </summary>
public sealed class Drug
{
    /// <summary>
    /// Unikalny identyfikator leku
    /// </summary>
    public Guid Id { get; private set; }
    
    /// <summary>
    /// Identyfikator właściciela leku (gospodarza domu)
    /// </summary>
    public Guid UserId { get; private set; }
    
    /// <summary>
    /// Nazwa leku
    /// </summary>
    public string Name { get; private set; } = string.Empty;
    
    /// <summary>
    /// Identyfikator systemowej jednostki głównej
    /// </summary>
    public Guid PrimaryUnitId { get; private set; }
    
    private readonly List<string> _barcodes = new();
    
    /// <summary>
    /// Lista kodów kreskowych przypisanych do leku
    /// </summary>
    public IReadOnlyList<string> Barcodes => _barcodes.AsReadOnly();
    
    /// <summary>
    /// Czy lek jest zarchiwizowany
    /// </summary>
    public bool IsArchived { get; private set; }
    
    /// <summary>
    /// Data utworzenia leku
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; }
    
    /// <summary>
    /// Data ostatniej aktualizacji leku
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; private set; }
    
    /// <summary>
    /// Data archiwizacji leku
    /// </summary>
    public DateTimeOffset? ArchivedAt { get; private set; }
    
    private readonly List<DerivedUnit> _derivedUnits = new();
    
    /// <summary>
    /// Lista jednostek pochodnych zdefiniowanych dla tego leku
    /// </summary>
    public IReadOnlyList<DerivedUnit> DerivedUnits => _derivedUnits.AsReadOnly();
    
    /// <summary>
    /// Systemowa jednostka główna (navigation property)
    /// </summary>
    public SystemUnit PrimaryUnit { get; private set; } = null!;
    
    /// <summary>
    /// Konstruktor bezparametrowy dla EF Core
    /// </summary>
    private Drug()
    {
    }
    
    /// <summary>
    /// Tworzy nową instancję leku
    /// </summary>
    /// <param name="userId">Identyfikator właściciela</param>
    /// <param name="name">Nazwa leku</param>
    /// <param name="primaryUnitId">Identyfikator jednostki głównej</param>
    /// <param name="barcodes">Lista kodów kreskowych</param>
    /// <returns>Nowa instancja Drug</returns>
    public static Drug Create(Guid userId, string name, Guid primaryUnitId, List<string>? barcodes)
    {
        var drug = new Drug
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = name,
            PrimaryUnitId = primaryUnitId,
            IsArchived = false,
            CreatedAt = DateTimeOffset.UtcNow
        };
        
        if (barcodes is not null)
        {
            drug._barcodes.AddRange(barcodes);
        }
        
        return drug;
    }
    
    /// <summary>
    /// Aktualizuje podstawowe informacje o leku
    /// </summary>
    /// <param name="name">Nowa nazwa leku</param>
    /// <param name="barcodes">Nowa lista kodów kreskowych</param>
    public void Update(string? name, List<string>? barcodes)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            Name = name;
        }
        
        _barcodes.Clear();
        if (barcodes is not null)
        {
            _barcodes.AddRange(barcodes);
        }
        
        UpdatedAt = DateTimeOffset.UtcNow;
    }
    
    /// <summary>
    /// Dodaje jednostkę pochodną do leku
    /// </summary>
    /// <param name="name">Nazwa jednostki</param>
    /// <param name="baseUnitId">Identyfikator jednostki bazowej</param>
    /// <param name="conversionFactor">Przelicznik do jednostki bazowej</param>
    /// <param name="isDefault">Czy to domyślna jednostka zakupu</param>
    /// <param name="barcodes">Lista kodów kreskowych</param>
    public void AddDerivedUnit(string name, Guid baseUnitId, decimal conversionFactor, bool isDefault, List<string>? barcodes)
    {
        var derivedUnit = DerivedUnit.Create(Id, name, baseUnitId, conversionFactor, isDefault, barcodes);
        _derivedUnits.Add(derivedUnit);
        UpdatedAt = DateTimeOffset.UtcNow;
    }
    
    /// <summary>
    /// Aktualizuje istniejącą jednostkę pochodną
    /// </summary>
    /// <param name="unitId">Identyfikator jednostki do aktualizacji</param>
    /// <param name="name">Nowa nazwa jednostki</param>
    /// <param name="conversionFactor">Nowy przelicznik</param>
    /// <param name="isDefault">Czy to domyślna jednostka zakupu</param>
    /// <param name="barcodes">Nowa lista kodów kreskowych</param>
    public void UpdateDerivedUnit(Guid unitId, string name, decimal conversionFactor, bool isDefault, List<string>? barcodes)
    {
        var unit = _derivedUnits.FirstOrDefault(u => u.Id == unitId);
        if (unit is not null)
        {
            unit.Update(name, conversionFactor, isDefault, barcodes);
            UpdatedAt = DateTimeOffset.UtcNow;
        }
    }
    
    /// <summary>
    /// Usuwa jednostkę pochodną z leku
    /// </summary>
    /// <param name="unitId">Identyfikator jednostki do usunięcia</param>
    public void RemoveDerivedUnit(Guid unitId)
    {
        var unit = _derivedUnits.FirstOrDefault(u => u.Id == unitId);
        if (unit is not null)
        {
            _derivedUnits.Remove(unit);
            UpdatedAt = DateTimeOffset.UtcNow;
        }
    }
    
    /// <summary>
    /// Archiwizuje lek (soft delete)
    /// </summary>
    public void Archive()
    {
        IsArchived = true;
        ArchivedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
    
    /// <summary>
    /// Przywraca zarchiwizowany lek
    /// </summary>
    public void Restore()
    {
        IsArchived = false;
        ArchivedAt = null;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
    
    /// <summary>
    /// Oblicza przelicznik jednostki pochodnej do jednostki głównej
    /// </summary>
    /// <param name="derivedUnitId">Identyfikator jednostki pochodnej</param>
    /// <returns>Przelicznik do jednostki głównej</returns>
    public decimal CalculateConversionToMain(Guid derivedUnitId)
    {
        // Znajdź jednostkę pochodną
        var unit = _derivedUnits.FirstOrDefault(u => u.Id == derivedUnitId);
        if (unit is null)
        {
            return 1m;
        }
        
        // Jeśli jednostka bazowa to jednostka główna, zwróć bezpośredni przelicznik
        if (unit.BaseUnitId == PrimaryUnitId)
        {
            return unit.ConversionFactor;
        }
        
        // Rekurencyjnie oblicz przelicznik dla jednostki bazowej
        return unit.ConversionFactor * CalculateConversionToMain(unit.BaseUnitId);
    }
}
