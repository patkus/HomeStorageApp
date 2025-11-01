# API Endpoint Implementation Plan: Drugs Management

## Analiza: SystemUnit vs DerivedUnit - Rozdzielenie czy połączenie?

### Za rozdzieleniem (obecne podejście)

**Zalety:**
1. **Type Safety** - Wyraźne rozróżnienie między systemowymi a użytkownikami zdefiniowanymi jednostkami
2. **Business Rules** - Różne reguły biznesowe:
   - SystemUnit: niemodyfikowalne, globalne, predefiniowane
   - DerivedUnit: edytowalne, należą do konkretnego leku, tworzone przez użytkownika
3. **Separation of Concerns** - Jasny podział odpowiedzialności
4. **Lifecycle Management** - Różne cykle życia (SystemUnit zawsze istnieją, DerivedUnit mogą być usuwane)
5. **Data Ownership** - SystemUnit są współdzielone, DerivedUnit należą do konkretnego Drug
6. **Czytelność kodu** - Intencja jest jasna w kodzie (`drug.PrimaryUnit` vs `drug.DerivedUnits`)

**Wady:**
1. Podobna struktura danych (Id, Name, Symbol/Code)
2. Potencjalna duplikacja logiki
3. Więcej klas do zarządzania

### Przeciw rozdzieleniu (jedno Unit)

**Zalety:**
1. **DRY Principle** - Jedna klasa zamiast dwóch
2. Mniej tabel w bazie
3. Prostszy model danych
4. Możliwość polimorficznego traktowania

**Wady:**
1. **Nullable Hell** - Wiele pól opcjonalnych (`DrugId?`, `BaseUnitId?`, `ConversionFactor?`)
2. **Słaba Type Safety** - Trudniej wymusić reguły biznesowe
3. **Walidacja** - Złożona logika walidacji (jeśli IsSystem to nie może mieć DrugId, etc.)
4. **Confusion** - Mniej czytelny kod (`unit.IsSystem ? ... : ...`)
5. **Naruszenie SRP** - Jedna klasa z dwiema odpowiedzialnościami

### Rekomendacja: ROZDZIELENIE ✅

**Decyzja:** Zachowujemy rozdzielenie `SystemUnit` i `DerivedUnit`

**Uzasadnienie:**
- Type safety i czytelność > DRY w przypadku różnych domen biznesowych
- Domain-Driven Design preferuje wyraźne rozdzielenie konceptów biznesowych
- Łatwiejsze w maintainowaniu i rozwijaniu
- Lepsze wymuszanie reguł biznesowych na poziomie typu

**Kompromis:**
- Możemy stworzyć bazowy interface `IUnit` dla współdzielonej logiki
- Extension methods dla wspólnych operacji
- Mappers mogą mieć wspólną bazę

```csharp
/// <summary>
/// Bazowy interfejs dla jednostek miar
/// </summary>
public interface IUnit
{
    Guid Id { get; }
    string Name { get; }
    string? Symbol { get; }
}

/// <summary>
/// Systemowa jednostka miary (predefiniowana, niemodyfikowalna)
/// </summary>
public sealed class SystemUnit : IUnit
{
    // Implementation
}

/// <summary>
/// Jednostka pochodna specyficzna dla leku (definiowana przez użytkownika)
/// </summary>
public sealed class DerivedUnit : IUnit
{
    // Implementation with additional Drug-specific fields
}
```

---

## 1. Przegląd punktu końcowego

Moduł Drugs API odpowiada za zarządzanie definicjami leków w systemie HomeStorageApp. Obejmuje pełny cykl CRUD (Create, Read, Update, Delete/Archive) oraz funkcjonalność przywracania zarchiwizowanych leków. Każdy lek jest powiązany z systemową jednostką główną (primary unit) i może posiadać własne jednostki pochodne z przelicznikami. System wspiera paginację, filtrowanie i sortowanie przy pobieraniu list leków.

**Główne funkcjonalności:**
- Przeglądanie listy aktywnych leków z sumarycznym stanem magazynowym
- Wyświetlanie szczegółów pojedynczego leku wraz z jednostkami miar
- Tworzenie nowych definicji leków z jednostką główną i pochodnymi
- Aktualizacja definicji (z ograniczeniami dla jednostki głównej)
- Archiwizacja (soft delete) leków
- Przywracanie zarchiwizowanych leków

## 2. Szczegóły żądania

### GET /api/drugs
**Metoda HTTP:** GET  
**Opis:** Pobiera stronicowaną listę aktywnych leków z podstawowymi informacjami i stanem magazynowym

**Parametry zapytania:**
- `page` (opcjonalny, int, default: 1) - Numer strony
- `pageSize` (opcjonalny, int, default: 20, max: 100) - Liczba elementów na stronę
- `sort` (opcjonalny, string) - Pole sortowania (np. "name", "createdAt", "totalStock")
- `filter` (opcjonalny, string) - Filtr nazwy leku (częściowe dopasowanie)

**Nagłówki:**
- `Authorization: Bearer {token}` (wymagany)

**Request Body:** Brak

---

### GET /api/drugs/{drugId}
**Metoda HTTP:** GET  
**Opis:** Pobiera szczegółowe informacje o pojedynczym leku

**Parametry URL:**
- `drugId` (wymagany, Guid) - Identyfikator leku

**Nagłówki:**
- `Authorization: Bearer {token}` (wymagany)

**Request Body:** Brak

---

### POST /api/drugs
**Metoda HTTP:** POST  
**Opis:** Tworzy nową definicję leku

**Nagłówki:**
- `Authorization: Bearer {token}` (wymagany)
- `Content-Type: application/json`

**Request Body:**
```json
{
  "name": "string (wymagane, max 200 znaków)",
  "primaryUnitId": "guid (wymagane) - ID systemowej jednostki głównej",
  "barcodes": ["string (opcjonalne, max 100 znaków każdy)] - lista kodów kreskowych dla leku",
  "derivedUnits": [
    {
      "name": "string (wymagane, max 100 znaków)",
      "baseUnitId": "guid (wymagane) - ID jednostki bazowej (główna lub inna pochodna)",
      "conversionFactor": "decimal (wymagane, > 0) - przelicznik do jednostki bazowej",
      "isDefaultPurchaseUnit": "boolean (opcjonalne, default: false)",
      "barcodes": ["string (opcjonalne)] - lista kodów kreskowych dla tej jednostki"
    }
  ]
}
```

---

### PUT /api/drugs/{drugId}
**Metoda HTTP:** PUT  
**Opis:** Aktualizuje istniejącą definicję leku

**Parametry URL:**
- `drugId` (wymagany, Guid) - Identyfikator leku

**Nagłówki:**
- `Authorization: Bearer {token}` (wymagany)
- `Content-Type: application/json`

**Request Body:**
```json
{
  "name": "string (opcjonalne, max 200 znaków)",
  "barcodes": ["string (opcjonalne, max 100 znaków każdy)] - lista kodów kreskowych",
  "derivedUnits": [
    {
      "id": "guid (opcjonalne) - jeśli istnieje, aktualizuje; jeśli brak, tworzy nową",
      "name": "string (wymagane, max 100 znaków)",
      "baseUnitId": "guid (wymagane)",
      "conversionFactor": "decimal (wymagane, > 0)",
      "isDefaultPurchaseUnit": "boolean (opcjonalne)",
      "barcodes": ["string (opcjonalne)] - lista kodów kreskowych"
    }
  ]
}
```

**Uwaga:** Jednostka główna (primaryUnit) nie może być zmieniona, jeśli istnieją zapasy leku.

---

### DELETE /api/drugs/{drugId}
**Metoda HTTP:** DELETE  
**Opis:** Archiwizuje (dezaktywuje) lek - soft delete

**Parametry URL:**
- `drugId` (wymagany, Guid) - Identyfikator leku

**Nagłówki:**
- `Authorization: Bearer {token}` (wymagany)

**Request Body:** Brak

---

### POST /api/drugs/{drugId}/restore
**Metoda HTTP:** POST  
**Opis:** Przywraca zarchiwizowany lek wraz z jego historią zapasów

**Parametry URL:**
- `drugId` (wymagany, Guid) - Identyfikator leku

**Nagłówki:**
- `Authorization: Bearer {token}` (wymagany)

**Request Body:** Brak

## 3. Wykorzystywane typy

### DTOs (Data Transfer Objects)

```csharp
/// <summary>
/// Request DTO dla tworzenia nowego leku
/// </summary>
public sealed record CreateDrugRequest(
    string Name,
    Guid PrimaryUnitId,
    List<string>? Barcodes,
    List<CreateDerivedUnitDto>? DerivedUnits);

/// <summary>
/// Request DTO dla tworzenia jednostki pochodnej
/// </summary>
public sealed record CreateDerivedUnitDto(
    string Name,
    Guid BaseUnitId,
    decimal ConversionFactor,
    bool IsDefaultPurchaseUnit,
    List<string>? Barcodes);

/// <summary>
/// Request DTO dla aktualizacji leku
/// </summary>
public sealed record UpdateDrugRequest(
    string? Name,
    List<string>? Barcodes,
    List<UpdateDerivedUnitDto>? DerivedUnits);

/// <summary>
/// Request DTO dla aktualizacji jednostki pochodnej
/// </summary>
public sealed record UpdateDerivedUnitDto(
    Guid? Id,
    string Name,
    Guid BaseUnitId,
    decimal ConversionFactor,
    bool IsDefaultPurchaseUnit,
    List<string>? Barcodes);

/// <summary>
/// Response DTO dla listy leków
/// </summary>
public sealed record DrugListResponse(
    Guid Id,
    string Name,
    string PrimaryUnitName,
    decimal TotalStock,
    DateOnly? NearestExpiryDate,
    int DerivedUnitsCount,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);

/// <summary>
/// Response DTO dla szczegółów leku
/// </summary>
public sealed record DrugDetailResponse(
    Guid Id,
    string Name,
    Guid PrimaryUnitId,
    string PrimaryUnitName,
    string? Barcode,
    bool IsArchived,
    List<DerivedUnitDto> DerivedUnits,
    decimal TotalStock,
    DateOnly? NearestExpiryDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);

/// <summary>
/// Response DTO dla jednostki pochodnej
/// </summary>
public sealed record DerivedUnitDto(
    Guid Id,
    string Name,
    Guid BaseUnitId,
    string BaseUnitName,
    decimal ConversionFactor,
    decimal ConversionToMain,
    bool IsDefaultPurchaseUnit,
    List<string> Barcodes);

/// <summary>
/// Response DTO dla stronicowanej listy
/// </summary>
public sealed record PaginatedResponse<T>(
    List<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
```

### Commands

```csharp
/// <summary>
/// Command do tworzenia nowego leku
/// </summary>
public sealed record CreateDrugCommand(
    Guid UserId,
    string Name,
    Guid PrimaryUnitId,
    List<string>? Barcodes,
    List<CreateDerivedUnitDto>? DerivedUnits);

/// <summary>
/// Command do aktualizacji leku
/// </summary>
public sealed record UpdateDrugCommand(
    Guid UserId,
    Guid DrugId,
    string? Name,
    List<string>? Barcodes,
    List<UpdateDerivedUnitDto>? DerivedUnits);

/// <summary>
/// Command do archiwizacji leku
/// </summary>
public sealed record ArchiveDrugCommand(
    Guid UserId,
    Guid DrugId);

/// <summary>
/// Command do przywracania leku
/// </summary>
public sealed record RestoreDrugCommand(
    Guid UserId,
    Guid DrugId);

/// <summary>
/// Query do pobierania listy leków
/// </summary>
public sealed record GetDrugsQuery(
    Guid UserId,
    int Page,
    int PageSize,
    string? SortBy,
    string? FilterName);

/// <summary>
/// Query do pobierania szczegółów leku
/// </summary>
public sealed record GetDrugByIdQuery(
    Guid UserId,
    Guid DrugId,
    bool IncludeArchived = false);
```

### Domain Entities

```csharp
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
    public string Name { get; private set; }
    
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
    /// Partie magazynowe leku (navigation property)
    /// </summary>
    public ICollection<StockBatch> StockBatches { get; private set; } = new List<StockBatch>();
    
    /// <summary>
    /// Konstruktor bezparametrowy dla EF Core
    /// </summary>
    private Drug() { }
    
    /// <summary>
    /// Tworzy nową instancję leku
    /// </summary>
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
    public void AddDerivedUnit(string name, Guid baseUnitId, decimal conversionFactor, bool isDefault, List<string>? barcodes)
    {
        var derivedUnit = DerivedUnit.Create(Id, name, baseUnitId, conversionFactor, isDefault, barcodes);
        _derivedUnits.Add(derivedUnit);
        UpdatedAt = DateTimeOffset.UtcNow;
    }
    
    /// <summary>
    /// Aktualizuje istniejącą jednostkę pochodną
    /// </summary>
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
    /// Sprawdza czy można zmienić jednostkę główną (możliwe tylko gdy brak zapasów)
    /// </summary>
    public bool CanChangePrimaryUnit() => !StockBatches.Any(b => b.Quantity > 0);
    
    /// <summary>
    /// Oblicza przelicznik jednostki pochodnej do jednostki głównej
    /// </summary>
    public decimal CalculateConversionToMain(Guid derivedUnitId)
    {
        // Implementacja rekurencyjnego obliczania przelicznika
        var unit = _derivedUnits.FirstOrDefault(u => u.Id == derivedUnitId);
        if (unit is null)
        {
            return 1m;
        }
        
        if (unit.BaseUnitId == PrimaryUnitId)
        {
            return unit.ConversionFactor;
        }
        
        return unit.ConversionFactor * CalculateConversionToMain(unit.BaseUnitId);
    }
}

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
    private DerivedUnit() { }
    
    /// <summary>
    /// Tworzy nową jednostkę pochodną
    /// </summary>
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
    private SystemUnit() { }
}

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
```

### Command Handlers (przykład zgodny z zasadami kodowania)

```csharp
/// <summary>
/// Handler dla komendy tworzenia leku
/// </summary>
public sealed class CreateDrugCommandHandler(
    IDrugRepository drugRepository,
    ISystemUnitRepository systemUnitRepository,
    ILogger<CreateDrugCommandHandler> logger)
{
    /// <summary>
    /// Obsługuje komendę tworzenia nowego leku
    /// </summary>
    public async Task<DrugDetailResponse> HandleAsync(
        CreateDrugCommand command,
        CancellationToken cancellationToken = default)
    {
        // Walidacja istnienia SystemUnit
        var systemUnit = await systemUnitRepository.GetByIdAsync(command.PrimaryUnitId, cancellationToken);
        if (systemUnit is null)
        {
            throw new ValidationException("PrimaryUnitId", "Wybrana jednostka nie istnieje");
        }
        
        // Sprawdzenie unikalności barcode
        if (!string.IsNullOrWhiteSpace(command.Barcode))
        {
            var exists = await drugRepository.ExistsByBarcodeAsync(command.Barcode, null, cancellationToken);
            if (exists)
            {
                throw new ConflictException($"Lek z kodem kreskowym '{command.Barcode}' już istnieje");
            }
        }
        
        // Utworzenie encji Drug
        var drug = Drug.Create(command.UserId, command.Name, command.PrimaryUnitId, command.Barcode);
        
        // Dodanie jednostek pochodnych
        if (command.DerivedUnits is not null)
        {
            foreach (var unitDto in command.DerivedUnits)
            {
                drug.AddDerivedUnit(
                    unitDto.Name,
                    unitDto.BaseUnitId,
                    unitDto.ConversionFactor,
                    unitDto.IsDefaultPurchaseUnit,
                    unitDto.Barcodes);
            }
        }
        
        // Zapis do bazy
        await drugRepository.AddAsync(drug, cancellationToken);
        await drugRepository.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation(
            "Drug {DrugId} created by user {UserId} with name {DrugName}",
            drug.Id, command.UserId, drug.Name);
        
        // Zwrócenie response
        return MapToDetailResponse(drug);
    }
    
    /// <summary>
    /// Mapuje encję Drug na DrugDetailResponse
    /// </summary>
    private static DrugDetailResponse MapToDetailResponse(Drug drug)
    {
        return new DrugDetailResponse(
            drug.Id,
            drug.Name,
            drug.PrimaryUnitId,
            drug.PrimaryUnit.Name,
            drug.Barcode,
            drug.IsArchived,
            drug.DerivedUnits.Select(u => new DerivedUnitDto(
                u.Id,
                u.Name,
                u.BaseUnitId,
                string.Empty, // BaseUnitName - należy pobrać
                u.ConversionFactor,
                drug.CalculateConversionToMain(u.Id),
                u.IsDefaultPurchaseUnit,
                u.Barcodes.ToList()
            )).ToList(),
            0m, // TotalStock - należy obliczyć
            null, // NearestExpiryDate - należy obliczyć
            drug.CreatedAt,
            drug.UpdatedAt);
    }
}
```

## 4. Szczegóły odpowiedzi

### GET /api/drugs - 200 OK
```json
{
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "Apap",
      "primaryUnitName": "tabletka",
      "totalStock": 250.0,
      "nearestExpiryDate": "2025-12-31T00:00:00Z",
      "derivedUnitsCount": 2,
      "createdAt": "2025-01-01T10:00:00Z",
      "updatedAt": "2025-01-15T14:30:00Z"
    }
  ],
  "page": 1,
  "pageSize": 20,
  "totalCount": 45,
  "totalPages": 3
}
```

### GET /api/drugs/{drugId} - 200 OK
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Apap",
  "primaryUnitId": "1fa85f64-5717-4562-b3fc-2c963f66afa1",
  "primaryUnitName": "tabletka",
  "barcode": "5901234567890",
  "isArchived": false,
  "derivedUnits": [
    {
      "id": "2fa85f64-5717-4562-b3fc-2c963f66afa2",
      "name": "blister",
      "baseUnitId": "1fa85f64-5717-4562-b3fc-2c963f66afa1",
      "baseUnitName": "tabletka",
      "conversionFactor": 10.0,
      "conversionToMain": 10.0,
      "isDefaultPurchaseUnit": false
    },
    {
      "id": "2fa85f64-5717-4562-b3fc-2c963f66afa3",
      "name": "opakowanie",
      "baseUnitId": "2fa85f64-5717-4562-b3fc-2c963f66afa2",
      "baseUnitName": "blister",
      "conversionFactor": 5.0,
      "conversionToMain": 50.0,
      "isDefaultPurchaseUnit": true
    }
  ],
  "totalStock": 250.0,
  "nearestExpiryDate": "2025-12-31T00:00:00Z",
  "createdAt": "2025-01-01T10:00:00Z",
  "updatedAt": "2025-01-15T14:30:00Z"
}
```

### POST /api/drugs - 201 Created
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Apap",
  "primaryUnitId": "1fa85f64-5717-4562-b3fc-2c963f66afa1",
  "primaryUnitName": "tabletka",
  "barcode": "5901234567890",
  "isArchived": false,
  "derivedUnits": [],
  "totalStock": 0,
  "nearestExpiryDate": null,
  "createdAt": "2025-01-01T10:00:00Z",
  "updatedAt": null
}
```

**Location header:** `/api/drugs/3fa85f64-5717-4562-b3fc-2c963f66afa6`

### PUT /api/drugs/{drugId} - 200 OK
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Apap Extra",
  "primaryUnitId": "1fa85f64-5717-4562-b3fc-2c963f66afa1",
  "primaryUnitName": "tabletka",
  "barcode": "5901234567890",
  "isArchived": false,
  "derivedUnits": [
    {
      "id": "2fa85f64-5717-4562-b3fc-2c963f66afa2",
      "name": "blister",
      "baseUnitId": "1fa85f64-5717-4562-b3fc-2c963f66afa1",
      "baseUnitName": "tabletka",
      "conversionFactor": 10.0,
      "conversionToMain": 10.0,
      "isDefaultPurchaseUnit": true
    }
  ],
  "totalStock": 250.0,
  "nearestExpiryDate": "2025-12-31T00:00:00Z",
  "createdAt": "2025-01-01T10:00:00Z",
  "updatedAt": "2025-01-20T16:45:00Z"
}
```

### DELETE /api/drugs/{drugId} - 200 OK
```json
{
  "success": true,
  "message": "Lek został zarchiwizowany"
}
```

### POST /api/drugs/{drugId}/restore - 200 OK
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Apap",
  "primaryUnitId": "1fa85f64-5717-4562-b3fc-2c963f66afa1",
  "primaryUnitName": "tabletka",
  "barcode": "5901234567890",
  "isArchived": false,
  "derivedUnits": [],
  "totalStock": 100.0,
  "nearestExpiryDate": "2025-12-31T00:00:00Z",
  "createdAt": "2025-01-01T10:00:00Z",
  "updatedAt": "2025-01-25T09:15:00Z"
}
```

### Kody błędów

**400 Bad Request**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Name": ["Nazwa leku jest wymagana"],
    "PrimaryUnitId": ["Jednostka główna jest wymagana"],
    "ConversionFactor": ["Przelicznik musi być większy od 0"]
  }
}
```

**401 Unauthorized**
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401,
  "detail": "Brak lub nieprawidłowy token autoryzacji"
}
```

**404 Not Found**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "detail": "Lek o podanym ID nie został znaleziony"
}
```

**409 Conflict**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.8",
  "title": "Conflict",
  "status": 409,
  "detail": "Nie można zmienić jednostki głównej - istnieją zapasy leku"
}
```

**500 Internal Server Error**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.6.1",
  "title": "Internal Server Error",
  "status": 500,
  "detail": "Wystąpił nieoczekiwany błąd serwera"
}
```

## 5. Przepływ danych

### Architektura warstwowa

```
┌─────────────────────────────────────────────┐
│         Minimal API Endpoints               │
│   (HomeStorageApp.Drugs.Api/Endpoints)     │
│  - DrugEndpoints.cs                         │
└────────────────┬────────────────────────────┘
                 │
                 │ DTOs (Request/Response)
                 ▼
┌─────────────────────────────────────────────┐
│         Command/Query Handlers              │
│   (HomeStorageApp.Drugs.Core/Application)   │
│  - CreateDrugCommandHandler                 │
│  - UpdateDrugCommandHandler                 │
│  - GetDrugsQueryHandler                     │
└────────────────┬────────────────────────────┘
                 │
                 │ Commands/Queries
                 ▼
┌─────────────────────────────────────────────┐
│         Domain Services                     │
│   (HomeStorageApp.Drugs.Core/Domain)        │
│  - Drug (Entity)                            │
│  - DerivedUnit (Entity)                     │
│  - IDrugRepository (Interface)              │
└────────────────┬────────────────────────────┘
                 │
                 │ Repository Interface
                 ▼
┌─────────────────────────────────────────────┐
│         EF Core Repository                  │
│   (HomeStorageApp.Drugs.Persistence)        │
│  - DrugRepository                           │
│  - DrugsDbContext                           │
└────────────────┬────────────────────────────┘
                 │
                 │ SQL Queries
                 ▼
┌─────────────────────────────────────────────┐
│         PostgreSQL Database                 │
│  - drugs table                              │
│  - derived_units table                      │
│  - system_units table                       │
│  - stock_batches table (FK)                 │
└─────────────────────────────────────────────┘
```

### Szczegółowy przepływ dla GET /api/drugs

1. **Endpoint Layer** (DrugEndpoints.cs)
   - Odbiera żądanie HTTP GET
   - Waliduje parametry query (page, pageSize)
   - Ekstrahuje UserId z JWT token claims
   - Tworzy GetDrugsQuery

2. **Handler Layer** (GetDrugsQueryHandler)
   - Odbiera query z parametrami
   - Wywołuje repository z filtrowaniem
   - Dla każdego leku:
     - Oblicza totalStock (suma z stock_batches)
     - Znajduje nearestExpiryDate (MIN z stock_batches)
     - Liczy derived units
   - Mapuje Domain Entity → DTO
   - Zwraca PaginatedResponse

3. **Repository Layer** (DrugRepository)
   - Wykonuje query z Include dla:
     - PrimaryUnit (SystemUnit)
     - DerivedUnits
     - StockBatches (dla agregacji)
   - Filtruje IsArchived = false
   - Filtruje UserId
   - Wykonuje paginację
   - Zwraca IQueryable<Drug>

4. **Database Layer**
   - Wykonuje zoptymalizowane SQL JOIN
   - Zwraca dane

### Szczegółowy przepływ dla POST /api/drugs

1. **Endpoint Layer**
   - Odbiera CreateDrugRequest
   - Waliduje model (FluentValidation)
   - Ekstrahuje UserId
   - Tworzy CreateDrugCommand

2. **Handler Layer** (CreateDrugCommandHandler)
   - Sprawdza, czy SystemUnit exists
   - Sprawdza unikalność Barcode (jeśli podany)
   - Tworzy Drug entity przez factory method
   - Dla każdego DerivedUnit:
     - Waliduje BaseUnit exists
     - Oblicza recursive conversion to main unit
     - Dodaje do Drug
   - Zapisuje przez repository
   - Mapuje → DrugDetailResponse

3. **Repository Layer**
   - Dodaje Drug do DbSet
   - Wywołuje SaveChangesAsync
   - Zwraca utworzony Drug z ID

4. **Database Layer**
   - INSERT do drugs
   - INSERT do derived_units (jeśli są)
   - Commit transakcji

### Szczegółowy przepływ dla PUT /api/drugs/{drugId}

1. **Endpoint Layer**
   - Odbiera UpdateDrugRequest + drugId
   - Waliduje model
   - Ekstrahuje UserId
   - Tworzy UpdateDrugCommand

2. **Handler Layer** (UpdateDrugCommandHandler)
   - Pobiera Drug przez repository
   - Sprawdza ownership (UserId)
   - Aplikuje zmiany:
     - Name (jeśli podane)
     - Barcode (sprawdza unikalność)
   - Dla DerivedUnits:
     - Jeśli Id exists: Update
     - Jeśli Id null: Create new
     - Jeśli brak na liście ale był: Delete
   - Update timestamp
   - Zapisuje zmiany
   - Zwraca DrugDetailResponse

3. **Repository Layer**
   - Pobiera Drug z Include
   - Update entity
   - SaveChangesAsync (EF tracking)

4. **Database Layer**
   - UPDATE drugs
   - UPDATE/INSERT/DELETE derived_units
   - Commit

### Integracja z innymi modułami

**Stocks Module:**
- Drug.TotalStock pochodzi z agregacji StockBatches
- Drug.NearestExpiryDate z MIN(StockBatch.ExpiryDate)
- Przed DELETE/Archive sprawdzenie czy są aktywne partie

**Scanning Module:**
- Drug.Barcode używany do identyfikacji przy skanowaniu
- DefaultPurchaseUnit używana przy auto-add (US-403)

**Dosing Module:**
- DerivedUnits używane do definiowania dawek
- Conversion factors do obliczania redukcji stanu

## 6. Względy bezpieczeństwa

### Uwierzytelnianie i autoryzacja

**JWT Bearer Token:**
- Wszystkie endpointy wymagają ważnego JWT token
- Token zawiera UserId w claims (ClaimTypes.NameIdentifier)
- Token weryfikowany przez ASP.NET Core middleware

**Authorization Policy:**
```csharp
.RequireAuthorization("HouseholdOwner");
```

**Weryfikacja własności:**
- Każda operacja sprawdza, czy Drug.UserId == token.UserId
- Zapobiega dostępowi do leków innych użytkowników
- Implementacja w HandlerBase lub przez policy

### Walidacja danych wejściowych

**FluentValidation dla Request DTOs:**

```csharp
public class CreateDrugRequestValidator : AbstractValidator<CreateDrugRequest>
{
    public CreateDrugRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nazwa leku jest wymagana")
            .MaximumLength(200).WithMessage("Nazwa nie może być dłuższa niż 200 znaków")
            .Matches("^[a-zA-ZąćęłńóśźżĄĆĘŁŃÓŚŹŻ0-9 -]+$")
            .WithMessage("Nazwa zawiera niedozwolone znaki");
            
        RuleFor(x => x.PrimaryUnitId)
            .NotEmpty().WithMessage("Jednostka główna jest wymagana");
            
        RuleFor(x => x.Barcode)
            .MaximumLength(100)
            .Matches("^[0-9A-Z-]+$").When(x => !string.IsNullOrEmpty(x.Barcode))
            .WithMessage("Kod kreskowy zawiera niedozwolone znaki");
            
        RuleForEach(x => x.DerivedUnits).ChildRules(unit =>
        {
            unit.RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);
                
            unit.RuleFor(x => x.ConversionFactor)
                .GreaterThan(0)
                .WithMessage("Przelicznik musi być większy od 0");
        });
    }
}
```

**Walidacja na poziomie domeny:**
- Drug entity waliduje business rules
- Sprawdzenie czy można zmienić PrimaryUnit
- Walidacja circular dependencies w DerivedUnits
- Sprawdzenie unikalności Barcode

### Ochrona przed atakami

**SQL Injection:**
- EF Core używa parametryzowanych zapytań
- LINQ zapytania są bezpieczne

**XSS (Cross-Site Scripting):**
- Wszystkie dane wyjściowe są escapowane przez JSON serializer
- Walidacja input nie pozwala na HTML/JS w polach tekstowych

**Mass Assignment:**
- Używanie dedykowanych DTOs zapobiega mass assignment
- Domain entities mają private setters

**CSRF:**
- API jest stateless (JWT)
- Brak cookies = brak ryzyka CSRF dla API

**Rate Limiting:**
- Implementacja middleware do rate limiting
- Limit requestów per user (np. 100/minute)

### Logowanie bezpieczeństwa

**Audit log dla wrażliwych operacji:**
- CREATE drug → log creation
- UPDATE drug → log changes (old vs new values)
- DELETE/Archive → log with reason
- RESTORE → log restoration

**Strukturyzowane logowanie:**
```csharp
_logger.LogInformation(
    "Drug {DrugId} created by user {UserId} with name {DrugName}",
    drug.Id, userId, drug.Name);
    
_logger.LogWarning(
    "Failed to update drug {DrugId} - primary unit locked due to existing stock",
    drugId);
```

### Dane wrażliwe

**GDPR Compliance:**
- Drug data należy do User
- Soft delete zachowuje historię
- Możliwość hard delete na żądanie (GDPR right to erasure)

**Brak PII w Logs:**
- Nie logować pełnych wartości Barcode
- Log tylko IDs i generic messages

## 7. Obsługa błędów

### Scenariusze błędów i odpowiedzi

#### 1. Walidacja danych wejściowych (400 Bad Request)

**Scenariusz:** Nazwa leku pusta lub zbyt długa
```csharp
throw new ValidationException("Name", "Nazwa leku jest wymagana");
```

**Scenariusz:** PrimaryUnitId nie istnieje w systemie
```csharp
throw new ValidationException("PrimaryUnitId", "Wybrana jednostka nie istnieje");
```

**Scenariusz:** ConversionFactor <= 0
```csharp
throw new ValidationException("ConversionFactor", "Przelicznik musi być większy od 0");
```

**Scenariusz:** Circular dependency w derived units
```csharp
throw new ValidationException("DerivedUnits", "Wykryto zapętlenie w hierarchii jednostek");
```

**Scenariusz:** Duplikat nazwy derived unit dla tego leku
```csharp
throw new ValidationException("DerivedUnits", "Jednostka o tej nazwie już istnieje dla tego leku");
```

#### 2. Conflict (409 Conflict)

**Scenariusz:** Próba zmiany PrimaryUnit gdy istnieją zapasy
```csharp
if (drug.StockBatches.Any(b => b.Quantity > 0))
{
    throw new ConflictException("Nie można zmienić jednostki głównej - istnieją zapasy leku");
}
```

**Scenariusz:** Duplikat Barcode
```csharp
if (await _repository.ExistsByBarcodeAsync(barcode, excludeDrugId))
{
    throw new ConflictException($"Lek z kodem kreskowym '{barcode}' już istnieje");
}
```

**Scenariusz:** Próba archived drug when already archived
```csharp
if (drug.IsArchived)
{
    throw new ConflictException("Lek jest już zarchiwizowany");
}
```

#### 3. Not Found (404)

**Scenariusz:** Drug nie istnieje
```csharp
var drug = await _repository.GetByIdAsync(drugId, userId);
if (drug is null)
{
    throw new NotFoundException($"Lek o ID {drugId} nie został znaleziony");
}
```

**Scenariusz:** DerivedUnit nie istnieje dla tego leku
```csharp
var unit = drug.DerivedUnits.FirstOrDefault(u => u.Id == unitId);
if (unit is null)
{
    throw new NotFoundException($"Jednostka o ID {unitId} nie została znaleziona");
}
```

#### 4. Unauthorized (401)

**Scenariusz:** Brak lub invalid JWT token
```csharp
// Obsłużone przez middleware
return Results.Unauthorized();
```

#### 5. Forbidden (403)

**Scenariusz:** UserId nie jest właścicielem leku
```csharp
if (drug.UserId != userId)
{
    throw new ForbiddenException("Brak uprawnień do modyfikacji tego leku");
}
```

#### 6. Internal Server Error (500)

**Scenariusz:** Database connection error
```csharp
try
{
    await _repository.SaveChangesAsync();
}
catch (DbUpdateException ex)
{
    _logger.LogError(ex, "Database error while saving drug {DrugId}", drugId);
    throw new InternalServerException("Wystąpił błąd podczas zapisywania danych");
}
```

**Scenariusz:** Unexpected exception
```csharp
catch (Exception ex)
{
    _logger.LogError(ex, "Unexpected error in CreateDrugCommandHandler");
    throw new InternalServerException("Wystąpił nieoczekiwany błąd serwera");
}
```

### Global Exception Handler

```csharp
// Shared/Middleware/GlobalExceptionHandler.cs
public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = exception switch
        {
            ValidationException validationEx => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Error",
                Detail = validationEx.Message
            },
            NotFoundException notFoundEx => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Not Found",
                Detail = notFoundEx.Message
            },
            ConflictException conflictEx => new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflict",
                Detail = conflictEx.Message
            },
            ForbiddenException forbiddenEx => new ProblemDetails
            {
                Status = StatusCodes.Status403Forbidden,
                Title = "Forbidden",
                Detail = forbiddenEx.Message
            },
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = "Wystąpił nieoczekiwany błąd serwera"
            }
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        
        return true;
    }
}
```

## 8. Rozważania dotyczące wydajności

### Optymalizacja zapytań

**1. SELECT N+1 Problem:**
```csharp
// ❌ BAD - N+1 queries
var drugs = await _context.Drugs.ToListAsync();
foreach (var drug in drugs)
{
    var stocks = await _context.StockBatches
        .Where(s => s.DrugId == drug.Id)
        .ToListAsync();
}

// ✅ GOOD - Single query with Include
var drugs = await _context.Drugs
    .Include(d => d.PrimaryUnit)
    .Include(d => d.DerivedUnits)
    .Include(d => d.StockBatches)
    .ToListAsync();
```

**2. Projection dla list (GET /api/drugs):**
```csharp
// ✅ Pobierz tylko potrzebne pola
var drugs = await _context.Drugs
    .Where(d => !d.IsArchived && d.UserId == userId)
    .Select(d => new DrugListResponse(
        d.Id,
        d.Name,
        d.PrimaryUnit.Name,
        d.StockBatches.Sum(s => s.Quantity),
        d.StockBatches.Min(s => s.ExpiryDate),
        d.DerivedUnits.Count,
        d.CreatedAt,
        d.UpdatedAt
    ))
    .ToListAsync();
```

**3. Paginacja:**
```csharp
var totalCount = await query.CountAsync();
var items = await query
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();
```

**4. Indeksy bazy danych:**
```sql
-- drugs table
CREATE INDEX idx_drugs_user_id ON drugs(user_id);
CREATE INDEX idx_drugs_barcode ON drugs(barcode) WHERE barcode IS NOT NULL;
CREATE INDEX idx_drugs_is_archived ON drugs(is_archived);
CREATE INDEX idx_drugs_name ON drugs(name);

-- derived_units table
CREATE INDEX idx_derived_units_drug_id ON derived_units(drug_id);
CREATE INDEX idx_derived_units_base_unit_id ON derived_units(base_unit_id);

-- stock_batches table (dla agregacji)
CREATE INDEX idx_stock_batches_drug_id ON stock_batches(drug_id);
CREATE INDEX idx_stock_batches_expiry_date ON stock_batches(expiry_date);
```

### Caching

**1. Response caching dla GET /api/drugs:**
```csharp
app.MapGet("/api/drugs", GetDrugsAsync)
    .CacheOutput(policy => policy
        .Expire(TimeSpan.FromMinutes(5))
        .Tag("drugs-list"));
```

**2. Invalidation cache po zmianach:**
```csharp
// Po CREATE, UPDATE, DELETE, RESTORE
await _cacheInvalidator.InvalidateTagAsync("drugs-list");
```

**3. In-memory cache dla SystemUnits:**
```csharp
// SystemUnits są read-only i mogą być cached długo
private static readonly MemoryCache _systemUnitsCache = new();

public async Task<SystemUnit?> GetSystemUnitAsync(Guid id)
{
    return await _systemUnitsCache.GetOrCreateAsync(
        $"system-unit-{id}",
        async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24);
            return await _context.SystemUnits.FindAsync(id);
        });
}
```

### Asynchroniczne operacje

```csharp
// ✅ Wszystkie database operations async
public async Task<Drug?> GetByIdAsync(Guid id, Guid userId)
{
    return await _context.Drugs
        .Include(d => d.PrimaryUnit)
        .Include(d => d.DerivedUnits)
        .FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);
}

// ✅ Batch operations
public async Task ArchiveManyAsync(IEnumerable<Guid> drugIds, Guid userId)
{
    await _context.Drugs
        .Where(d => drugIds.Contains(d.Id) && d.UserId == userId)
        .ExecuteUpdateAsync(s => s
            .SetProperty(d => d.IsArchived, true)
            .SetProperty(d => d.ArchivedAt, DateTime.UtcNow));
}
```

### Connection pooling

```csharp
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=homestorage;Username=postgres;Password=***;Pooling=true;MinPoolSize=5;MaxPoolSize=100;ConnectionLifetime=300"
  }
}
```

### Monitoring

```csharp
// Metryki wydajnościowe
public class DrugQueryMetrics
{
    private readonly IMetrics _metrics;
    
    public async Task<PaginatedResponse<DrugListResponse>> GetDrugsWithMetrics(...)
    {
        using var timer = _metrics.Measure.Timer.Time(new TimerOptions
        {
            Name = "drugs_query_duration"
        });
        
        var result = await GetDrugs(...);
        
        _metrics.Measure.Counter.Increment(new CounterOptions
        {
            Name = "drugs_queries_total"
        });
        
        return result;
    }
}
```

## 9. Etapy wdrożenia

### Faza 1: Przygotowanie infrastruktury

1. **Utworzenie struktury modułu Drugs**
   ```
   src/Modules/Drugs/
   ├── HomeStorageApp.Drugs.Api/
   │   ├── Endpoints/
   │   │   └── DrugEndpoints.cs
   │   ├── Extensions/
   │   │   └── DrugsModuleExtensions.cs
   │   └── Validators/
   │       ├── CreateDrugRequestValidator.cs
   │       └── UpdateDrugRequestValidator.cs
   ├── HomeStorageApp.Drugs.Core/
   │   ├── Application/
   │   │   ├── Commands/
   │   │   │   ├── CreateDrug/
   │   │   │   ├── UpdateDrug/
   │   │   │   ├── ArchiveDrug/
   │   │   │   └── RestoreDrug/
   │   │   ├── Queries/
   │   │   │   ├── GetDrugs/
   │   │   │   └── GetDrugById/
   │   │   ├── DTOs/
   │   │   └── Interfaces/
   │   │       └── IDrugRepository.cs
   │   └── Domain/
   │       ├── Drugs/
   │       │   ├── Drug.cs
   │       │   └── DerivedUnit.cs
   │       ├── SystemUnits/
   │       │   └── SystemUnit.cs
   │       └── Exceptions/
   │           └── DrugDomainException.cs
   └── HomeStorageApp.Drugs.Persistence/
       ├── Configurations/
       │   ├── DrugConfiguration.cs
       │   ├── DerivedUnitConfiguration.cs
       │   └── SystemUnitConfiguration.cs
       ├── Repositories/
       │   └── DrugRepository.cs
       ├── Migrations/
       └── DrugsDbContext.cs
   ```

2. **Konfiguracja projekt references**
   - Drugs.Api → Drugs.Core
   - Drugs.Core → HomeStorageApp.Shared
   - Drugs.Persistence → Drugs.Core
   - HomeStorageApp → Drugs.Api, Drugs.Persistence

### Faza 2: Domain Layer

3. **Implementacja SystemUnit Entity**
   - Właściwości: Id, Name, Symbol, Category
   - Enum SystemUnitCategory
   - Seeding danych (tabletka, ml, g, etc.)

4. **Implementacja Drug Entity**
   - Właściwości podstawowe
   - Factory method Create()
   - Business methods (Update, Archive, Restore)
   - Navigation properties

5. **Implementacja DerivedUnit Entity**
   - Właściwości
   - Logika przeliczania conversion factors
   - Walidacja circular dependencies

6. **Domain Exceptions**
   - DrugDomainException base class
   - Specific exceptions (InvalidUnitException, etc.)

### Faza 3: Persistence Layer

7. **DbContext configuration**
   - DrugsDbContext z DbSets
   - OnModelCreating configuration

8. **Entity Configurations**
   - DrugConfiguration (FluentAPI)
   - DerivedUnitConfiguration
   - SystemUnitConfiguration
   - Relationships, indices, constraints

9. **Repository implementation**
   - IDrugRepository interface
   - DrugRepository concrete class
   - Methods: Get, GetById, Create, Update, Delete, etc.

10. **EF Core Migrations**
    ```bash
    dotnet ef migrations add InitialDrugsSchema -p src/Modules/Drugs/HomeStorageApp.Drugs.Persistence
    ```

11. **Seed SystemUnits data**
    - Migration z INSERT dla predefiniowanych jednostek
    - Lub DataSeeder class

### Faza 4: Application Layer

12. **DTOs implementation**
    - Request DTOs (CreateDrugRequest, UpdateDrugRequest)
    - Response DTOs (DrugListResponse, DrugDetailResponse, PaginatedResponse)
    - Internal DTOs (CreateDerivedUnitDto, etc.)

13. **Commands implementation**
    - CreateDrugCommand + CreateDrugCommandHandler
    - UpdateDrugCommand + UpdateDrugCommandHandler
    - ArchiveDrugCommand + ArchiveDrugCommandHandler
    - RestoreDrugCommand + RestoreDrugCommandHandler

14. **Queries implementation**
    - GetDrugsQuery + GetDrugsQueryHandler
    - GetDrugByIdQuery + GetDrugByIdQueryHandler

15. **Mapping logic**
    - Extensions methods dla mapowania Entity ↔ DTO
    - Lub AutoMapper profiles

### Faza 5: API Layer

16. **FluentValidation Validators**
    - CreateDrugRequestValidator
    - UpdateDrugRequestValidator
    - Registration w DI

17. **Minimal API Endpoints**
    - DrugEndpoints.cs z metodami:
      - MapGet, MapPost, MapPut, MapDelete
    - Swagger/OpenAPI documentation
    - Input validation
    - UserId extraction z JWT

18. **Module Extensions**
    - DrugsModuleExtensions.cs
    - AddDrugsModule() method
    - Registration services, validators, repositories

### Faza 6: Integration

19. **Registration w Program.cs**
    ```csharp
    builder.Services.AddDrugsModule(builder.Configuration);
    app.MapDrugsEndpoints();
    ```

20. **Database migration application**
    ```bash
    dotnet ef database update -p src/Modules/Drugs/HomeStorageApp.Drugs.Persistence
    ```

### Faza 7: Testing

21. **Unit Tests**
    - Domain logic tests
    - Handler tests (z mocked repository)
    - Validation tests

22. **Integration Tests**
    - API endpoints tests
    - Database integration tests
    - Test scenarios dla wszystkich happy paths i error cases

23. **Manual Testing**
    - Postman/Thunder Client collection
    - Test wszystkich endpointów
    - Verify responses, status codes

### Faza 8: Documentation & Deployment

24. **Documentation**
    - Swagger/OpenAPI complete
    - README w module folder
    - Architecture Decision Records (ADRs)

25. **Performance testing**
    - Load testing
    - Query performance analysis
    - Optimization jeśli potrzebne

26. **Deployment preparation**
    - Environment configurations
    - Connection strings
    - Logging configuration
    - Migration scripts

### Checklist końcowy wdrożenia

- [ ] Wszystkie testy jednostkowe przechodzą
- [ ] Wszystkie testy integracyjne przechodzą
- [ ] Swagger documentation kompletna i poprawna
- [ ] Security review przeprowadzony
- [ ] Performance benchmarks zadowalające
- [ ] Migration scripts przetestowane
- [ ] Rollback plan przygotowany
- [ ] Monitoring i logging skonfigurowane
- [ ] Documentation zaktualizowana
- [ ] Code review zaakceptowany

---

## Podsumowanie

Ten plan implementacji zapewnia kompleksowe wytyczne dla zespołu programistów do wdrożenia modułu Drugs API zgodnie z architekturą modularnego monolitu, wzorcami DDD i CQRS, oraz najlepszymi praktykami bezpieczeństwa i wydajności dla ASP.NET Core i EF Core z PostgreSQL.
