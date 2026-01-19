using Microsoft.EntityFrameworkCore;
using MyBankingApp.API.Middleware;
using MyBankingApp.Application;
using MyBankingApp.Infrastructure;
using MyBankingApp.Infrastructure.Persistence;
using Scalar.AspNetCore;
using Serilog;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add Serilog
    builder.Host.UseSerilog();

    // Add services to the container
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    // Add CORS for React frontend
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("ReactApp", policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });

    // Add controllers
    builder.Services.AddControllers();

    // Add OpenAPI (built-in .NET 10 support)
    builder.Services.AddOpenApi();

    var app = builder.Build();

    // Apply migrations and seed database
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<BankingDbContext>();
        context.Database.EnsureCreated();
    }

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options.Title = "MyBankingApp API";
            options.Theme = ScalarTheme.BluePlanet;
        });
    }

    // Add middleware
    app.UseMiddleware<ExceptionHandlingMiddleware>();
    
    app.UseCors("ReactApp");
    app.UseHttpsRedirection();
    app.MapControllers();

    Log.Information("Starting MyBankingApp API");
    Log.Information("API Documentation available at: http://localhost:5114/scalar/v1");
    
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}