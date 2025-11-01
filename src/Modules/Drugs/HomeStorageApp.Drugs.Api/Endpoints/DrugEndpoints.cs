using System.Security.Claims;
using FluentValidation;
using HomeStorageApp.Drugs.Core.Application.Commands.ArchiveDrug;
using HomeStorageApp.Drugs.Core.Application.Commands.CreateDrug;
using HomeStorageApp.Drugs.Core.Application.Commands.RestoreDrug;
using HomeStorageApp.Drugs.Core.Application.Commands.UpdateDrug;
using HomeStorageApp.Drugs.Core.Application.DTOs;
using HomeStorageApp.Drugs.Core.Application.Queries.GetDrugById;
using HomeStorageApp.Drugs.Core.Application.Queries.GetDrugs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace HomeStorageApp.Drugs.Api.Endpoints;

/// <summary>
/// Definiuje endpointy API dla modułu Drugs
/// </summary>
public static class DrugEndpoints
{
    /// <summary>
    /// Mapuje endpointy dla leków
    /// </summary>
    /// <param name="app">Builder aplikacji</param>
    public static void MapDrugEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/drugs")
            .WithTags("Drugs")
            .RequireAuthorization();

        // GET /api/drugs - Pobierz listę leków
        group.MapGet("/", GetDrugsAsync)
            .WithName("GetDrugs");

        // GET /api/drugs/{drugId} - Pobierz szczegóły leku
        group.MapGet("/{drugId:guid}", GetDrugByIdAsync)
            .WithName("GetDrugById");

        // POST /api/drugs - Utwórz nowy lek
        group.MapPost("/", CreateDrugAsync)
            .WithName("CreateDrug");

        // PUT /api/drugs/{drugId} - Zaktualizuj lek
        group.MapPut("/{drugId:guid}", UpdateDrugAsync)
            .WithName("UpdateDrug");

        // DELETE /api/drugs/{drugId} - Archiwizuj lek
        group.MapDelete("/{drugId:guid}", ArchiveDrugAsync)
            .WithName("ArchiveDrug");

        // POST /api/drugs/{drugId}/restore - Przywróć zarchiwizowany lek
        group.MapPost("/{drugId:guid}/restore", RestoreDrugAsync)
            .WithName("RestoreDrug");
    }

    /// <summary>
    /// Pobiera stronicowaną listę leków
    /// </summary>
    private static async Task<IResult> GetDrugsAsync(
        HttpContext context,
        GetDrugsQueryHandler handler,
        int page = 1,
        int pageSize = 20,
        string? sortBy = null,
        string? filterName = null,
        CancellationToken cancellationToken = default)
    {
        var userId = GetUserId(context);
        
        var query = new GetDrugsQuery(userId, page, pageSize, sortBy, filterName);
        var result = await handler.HandleAsync(query, cancellationToken);
        
        return Results.Ok(result);
    }

    /// <summary>
    /// Pobiera szczegóły leku po identyfikatorze
    /// </summary>
    private static async Task<IResult> GetDrugByIdAsync(
        Guid drugId,
        HttpContext context,
        GetDrugByIdQueryHandler handler,
        CancellationToken cancellationToken = default)
    {
        var userId = GetUserId(context);
        
        var query = new GetDrugByIdQuery(userId, drugId);
        var result = await handler.HandleAsync(query, cancellationToken);
        
        return Results.Ok(result);
    }

    /// <summary>
    /// Tworzy nowy lek
    /// </summary>
    private static async Task<IResult> CreateDrugAsync(
        CreateDrugRequest request,
        HttpContext context,
        CreateDrugCommandHandler handler,
        IValidator<CreateDrugRequest> validator,
        CancellationToken cancellationToken = default)
    {
        // Walidacja request
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }
        
        var userId = GetUserId(context);
        
        var command = new CreateDrugCommand(
            userId,
            request.Name,
            request.PrimaryUnitId,
            request.Barcodes,
            request.DerivedUnits);
            
        var result = await handler.HandleAsync(command, cancellationToken);
        
        return Results.Created($"/api/drugs/{result.Id}", result);
    }

    /// <summary>
    /// Aktualizuje istniejący lek
    /// </summary>
    private static async Task<IResult> UpdateDrugAsync(
        Guid drugId,
        UpdateDrugRequest request,
        HttpContext context,
        UpdateDrugCommandHandler handler,
        IValidator<UpdateDrugRequest> validator,
        CancellationToken cancellationToken = default)
    {
        // Walidacja request
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }
        
        var userId = GetUserId(context);
        
        var command = new UpdateDrugCommand(
            userId,
            drugId,
            request.Name,
            request.Barcodes,
            request.DerivedUnits);
            
        var result = await handler.HandleAsync(command, cancellationToken);
        
        return Results.Ok(result);
    }

    /// <summary>
    /// Archiwizuje lek (soft delete)
    /// </summary>
    private static async Task<IResult> ArchiveDrugAsync(
        Guid drugId,
        HttpContext context,
        ArchiveDrugCommandHandler handler,
        CancellationToken cancellationToken = default)
    {
        var userId = GetUserId(context);
        
        var command = new ArchiveDrugCommand(userId, drugId);
        await handler.HandleAsync(command, cancellationToken);
        
        return Results.Ok(new { success = true, message = "Lek został zarchiwizowany" });
    }

    /// <summary>
    /// Przywraca zarchiwizowany lek
    /// </summary>
    private static async Task<IResult> RestoreDrugAsync(
        Guid drugId,
        HttpContext context,
        RestoreDrugCommandHandler handler,
        CancellationToken cancellationToken = default)
    {
        var userId = GetUserId(context);
        
        var command = new RestoreDrugCommand(userId, drugId);
        var result = await handler.HandleAsync(command, cancellationToken);
        
        return Results.Ok(result);
    }

    /// <summary>
    /// Ekstrahuje UserId z JWT token claims
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <returns>Identyfikator użytkownika</returns>
    private static Guid GetUserId(HttpContext context)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Nie można zidentyfikować użytkownika");
        }
        return userId;
    }
}
