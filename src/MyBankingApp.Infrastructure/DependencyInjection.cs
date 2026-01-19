using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyBankingApp.Domain.Interfaces;
using MyBankingApp.Infrastructure.Persistence;
using MyBankingApp.Infrastructure.Persistence.Repositories;

namespace MyBankingApp.Infrastructure;

/// <summary>
/// Dependency injection extension for Infrastructure layer.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register DbContext
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        if (string.IsNullOrEmpty(connectionString))
        {
            // Use SQLite for development if no connection string provided
            services.AddDbContext<BankingDbContext>(options =>
                options.UseSqlite("Data Source=MyBankingApp.db"));
        }
        else
        {
            // Use SQL Server for production
            services.AddDbContext<BankingDbContext>(options =>
                options.UseSqlServer(connectionString));
        }

        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        
        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
