using FluentValidation;
using HomeStorageApp.Drugs.Api.Endpoints;
using HomeStorageApp.Drugs.Core.Application.Commands.ArchiveDrug;
using HomeStorageApp.Drugs.Core.Application.Commands.CreateDrug;
using HomeStorageApp.Drugs.Core.Application.Commands.RestoreDrug;
using HomeStorageApp.Drugs.Core.Application.Commands.UpdateDrug;
using HomeStorageApp.Drugs.Core.Application.Interfaces;
using HomeStorageApp.Drugs.Core.Application.Queries.GetDrugById;
using HomeStorageApp.Drugs.Core.Application.Queries.GetDrugs;
using HomeStorageApp.Drugs.Persistence;
using HomeStorageApp.Drugs.Persistence.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HomeStorageApp.Drugs.Api.Extensions;

/// <summary>
/// Rozszerzenia do rejestracji modułu Drugs
/// </summary>
public static class DrugsModuleExtensions
{
    /// <summary>
    /// Dodaje moduł Drugs do aplikacji
    /// </summary>
    /// <param name="services">Kolekcja serwisów</param>
    /// <param name="configuration">Konfiguracja aplikacji</param>
    /// <returns>Kolekcja serwisów</returns>
    public static IServiceCollection AddDrugsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Rejestracja DbContext
        services.AddDbContext<DrugsDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("HomeStorageApp.Drugs.Persistence")));

        // Rejestracja Repositories
        services.AddScoped<IDrugRepository, DrugRepository>();
        services.AddScoped<ISystemUnitRepository, SystemUnitRepository>();

        // Rejestracja Command Handlers
        services.AddScoped<CreateDrugCommandHandler>();
        services.AddScoped<UpdateDrugCommandHandler>();
        services.AddScoped<ArchiveDrugCommandHandler>();
        services.AddScoped<RestoreDrugCommandHandler>();

        // Rejestracja Query Handlers
        services.AddScoped<GetDrugsQueryHandler>();
        services.AddScoped<GetDrugByIdQueryHandler>();

        // Rejestracja FluentValidation
        services.AddValidatorsFromAssembly(typeof(DrugsModuleExtensions).Assembly);

        return services;
    }

    /// <summary>
    /// Mapuje endpointy modułu Drugs
    /// </summary>
    /// <param name="app">Builder aplikacji</param>
    /// <returns>Builder aplikacji</returns>
    public static IEndpointRouteBuilder MapDrugsModule(this IEndpointRouteBuilder app)
    {
        app.MapDrugEndpoints();
        return app;
    }
    
    /// <summary>
    /// Aplikuje automatyczne migracje dla modułu Drugs
    /// </summary>
    /// <param name="app">Aplikacja</param>
    public static async Task UseDrugsMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<DrugsDbContext>>();
        
        try
        {
            var context = services.GetRequiredService<DrugsDbContext>();
            await context.Database.MigrateAsync();
            logger.LogInformation("Drugs database migration completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the Drugs database");
            throw;
        }
    }
}
