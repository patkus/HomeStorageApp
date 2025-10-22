using HomeStorageApp.Shared.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HomeStorageApp.Shared.Middleware;

/// <summary>
/// Globalny handler wyjątków dla całej aplikacji.
/// Obsługuje wyjątki ze wszystkich modułów i mapuje je na odpowiednie HTTP responses.
/// Wykorzystuje wzorzec DomainException jako bazę dla wszystkich wyjątków domenowych.
/// </summary>
public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    /// <summary>
    /// Obsługuje wyjątki i zwraca odpowiednie HTTP responses.
    /// </summary>
    /// <param name="httpContext">HTTP context</param>
    /// <param name="exception">Wyjątek do obsługi</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>True, jeśli wyjątek został obsłużony, false w przeciwnym razie</returns>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Mapowanie wyjątku na status code i komunikat
        var (statusCode, title, detail) = MapException(exception);

        // Logowanie wyjątku
        logger.LogError(
            exception,
            "Exception occurred: {ExceptionType} - {Message}. Status: {Status}",
            exception.GetType().Name,
            exception.Message,
            statusCode);

        // Utworzenie ProblemDetails
        var problemDetails = new ProblemDetails
        {
            Type = GetProblemType(statusCode),
            Title = title,
            Status = statusCode,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        // Ustawienie status code i zwrócenie JSON
        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    /// <summary>
    /// Mapuje wyjątek na kod HTTP, tytuł i szczegóły błędu.
    /// Rozpoznaje wyjątki domenowe po nazwie klasy używając konwencji nazewnictwa.
    /// </summary>
    /// <param name="exception">Wyjątek do zmapowania</param>
    /// <returns>Tuple z kodem HTTP, tytułem i szczegółami</returns>
    private static (int StatusCode, string Title, string Detail) MapException(Exception exception)
    {
        // Sprawdź czy to wyjątek domenowy
        if (exception is DomainException domainException)
        {
            return MapDomainException(domainException);
        }

        // Mapowanie standardowych wyjątków .NET
        return exception switch
        {
            ArgumentNullException e => (
                StatusCodes.Status400BadRequest,
                "Błąd walidacji",
                $"Parametr '{e.ParamName}' nie może być pusty"),

            ArgumentException e => (
                StatusCodes.Status400BadRequest,
                "Błąd walidacji",
                e.Message),

            InvalidOperationException e => (
                StatusCodes.Status400BadRequest,
                "Nieprawidłowa operacja",
                e.Message),

            UnauthorizedAccessException e => (
                StatusCodes.Status401Unauthorized,
                "Nieautoryzowany",
                e.Message),

            // 500 Internal Server Error - Nieoczekiwane błędy
            _ => (
                StatusCodes.Status500InternalServerError,
                "Błąd serwera",
                "Wystąpił nieoczekiwany błąd. Spróbuj ponownie później.")
        };
    }

    /// <summary>
    /// Mapuje wyjątki domenowe na kody HTTP używając typowanych bazowych wyjątków.
    /// Wykorzystuje compile-time type checking dla bezpieczeństwa i czytelności.
    /// </summary>
    /// <param name="exception">Wyjątek domenowy</param>
    /// <returns>Tuple z kodem HTTP, tytułem i szczegółami</returns>
    private static (int StatusCode, string Title, string Detail) MapDomainException(DomainException exception)
    {
        return exception switch
        {
            // 409 Conflict - Zasób już istnieje lub operacja powoduje konflikt
            ConflictException e => (
                StatusCodes.Status409Conflict,
                "Konflikt",
                e.Message),

            // 404 Not Found - Zasób nie został znaleziony
            NotFoundException e => (
                StatusCodes.Status404NotFound,
                "Nie znaleziono",
                e.Message),

            // 401 Unauthorized - Nieprawidłowe uwierzytelnienie
            UnauthorizedException e => (
                StatusCodes.Status401Unauthorized,
                "Nieautoryzowany",
                e.Message),

            // 403 Forbidden - Brak uprawnień do wykonania operacji
            ForbiddenException e => (
                StatusCodes.Status403Forbidden,
                "Dostęp zabroniony",
                e.Message),

            // 400 Bad Request - Błąd walidacji danych wejściowych
            ValidationException e => (
                StatusCodes.Status400BadRequest,
                "Błąd walidacji",
                e.Message),

            // 400 Bad Request - Domyślny dla innych wyjątków domenowych
            _ => (
                StatusCodes.Status400BadRequest,
                "Błąd żądania",
                exception.Message)
        };
    }

    /// <summary>
    /// Zwraca URI typu problemu zgodny z RFC 9110.
    /// </summary>
    /// <param name="statusCode">Kod statusu HTTP</param>
    /// <returns>URI typu problemu</returns>
    private static string GetProblemType(int statusCode)
    {
        // Oblicz numer sekcji RFC na podstawie kodu statusu
        // np. 400 -> 15.5.1, 401 -> 15.5.2, itd.
        var section = statusCode switch
        {
            >= 400 and < 500 => $"15.5.{statusCode - 399}",
            >= 500 => $"15.6.{statusCode - 499}",
            _ => "15"
        };

        return $"https://tools.ietf.org/html/rfc9110#section-{section}";
    }
}
