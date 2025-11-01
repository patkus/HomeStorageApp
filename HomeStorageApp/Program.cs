using HomeStorageApp.Identity.Api.Endpoints;
using HomeStorageApp.Identity.Api.Extensions;
using HomeStorageApp.Drugs.Api.Extensions;
using HomeStorageApp.Shared.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddOpenApi();

// Add Identity Module
builder.Services.AddIdentityModule(builder.Configuration);

// Add Drugs Module
builder.Services.AddDrugsModule(builder.Configuration);

// Add Global Exception Handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Automatyczne migracje baz danych
await app.UseIdentityMigrationsAsync();
await app.UseDrugsMigrationsAsync();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Global Exception Handler (musi być przed innymi middleware)
app.UseExceptionHandler();

app.UseHttpsRedirection();

// Authentication & Authorization (musi być w tej kolejności)
app.UseAuthentication();
app.UseAuthorization();

// Map Identity endpoints
app.MapAuthEndpoints();

// Map Drugs endpoints
app.MapDrugsModule();

app.Run();
