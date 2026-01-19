using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyBankingApp.Domain.Entities;

namespace MyBankingApp.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for Transaction entity.
/// </summary>
public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.ReferenceNumber)
            .IsRequired()
            .HasMaxLength(30);

        builder.HasIndex(t => t.ReferenceNumber)
            .IsUnique();

        builder.Property(t => t.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(t => t.Description)
            .HasMaxLength(500);

        // Configure Amount as owned type
        builder.OwnsOne(t => t.Amount, amount =>
        {
            amount.Property(m => m.Amount)
                .HasColumnName("Amount")
                .HasPrecision(18, 2)
                .IsRequired();

            amount.Property(m => m.Currency)
                .HasColumnName("AmountCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        // Configure BalanceAfter as owned type
        builder.OwnsOne(t => t.BalanceAfter, balance =>
        {
            balance.Property(m => m.Amount)
                .HasColumnName("BalanceAfter")
                .HasPrecision(18, 2)
                .IsRequired();

            balance.Property(m => m.Currency)
                .HasColumnName("BalanceAfterCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.HasIndex(t => t.AccountId);
        builder.HasIndex(t => t.CreatedAt);

        // Ignore domain events
        builder.Ignore(t => t.DomainEvents);
    }
}
