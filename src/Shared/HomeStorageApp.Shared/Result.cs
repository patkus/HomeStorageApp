namespace HomeStorageApp.Shared;

/// <summary>
/// Reprezentuje wynik operacji, która może zakończyć się sukcesem lub niepowodzeniem.
/// Używany do functional error handling zamiast rzucania wyjątków.
/// </summary>
/// <typeparam name="T">Typ wartości zwracanej w przypadku sukcesu</typeparam>
public sealed class Result<T>
{
    /// <summary>
    /// Określa, czy operacja zakończyła się sukcesem
    /// </summary>
    private bool IsSuccess { get; }
    
    /// <summary>
    /// Określa, czy operacja zakończyła się niepowodzeniem
    /// </summary>
    public bool IsFailure => !IsSuccess;
    
    /// <summary>
    /// Wartość zwracana w przypadku sukcesu
    /// </summary>
    public T Value { get; }
    
    /// <summary>
    /// Komunikat o błędzie w przypadku niepowodzenia
    /// </summary>
    public string Error { get; }

    /// <summary>
    /// Konstruktor prywatny-użyj metod fabrycznych Success lub Failure
    /// </summary>
    private Result(bool isSuccess, T value, string error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    /// <summary>
    /// Tworzy wynik reprezentujący sukces operacji
    /// </summary>
    /// <param name="value">Wartość zwracana przez operację</param>
    /// <returns>Obiekt Result reprezentujący sukces</returns>
    public static Result<T> Success(T value) => new(true, value, string.Empty);
    
    /// <summary>
    /// Tworzy wynik reprezentujący niepowodzenie operacji
    /// </summary>
    /// <param name="error">Komunikat błędu opisujący przyczynę niepowodzenia</param>
    /// <returns>Obiekt Result reprezentujący niepowodzenie</returns>
    public static Result<T> Failure(string error) => new(false, default!, error);
}
