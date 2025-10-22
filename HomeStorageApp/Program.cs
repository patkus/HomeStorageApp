using HomeStorageApp.Identity.Api.Endpoints;
using HomeStorageApp.Identity.Api.Extensions;
using HomeStorageApp.Shared.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddOpenApi();

// Add Identity Module
builder.Services.AddIdentityModule(builder.Configuration);

// Add Global Exception Handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

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

app.Run();
