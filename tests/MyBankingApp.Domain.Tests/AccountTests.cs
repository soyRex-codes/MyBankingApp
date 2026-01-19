using FluentAssertions;
using MyBankingApp.Domain.Entities;
using MyBankingApp.Domain.Enums;
using MyBankingApp.Domain.Exceptions;
using MyBankingApp.Domain.ValueObjects;

namespace MyBankingApp.Domain.Tests;

public class AccountTests
{
    [Fact]
    public void CreateAccount_ShouldInitializeWithZeroBalance()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        // Act
        var account = new Account(userId, AccountType.Checking);
        
        // Assert
        account.Balance.Amount.Should().Be(0);
        account.Status.Should().Be(AccountStatus.Active);
        account.UserId.Should().Be(userId);
        account.AccountNumber.Should().HaveLength(12);
    }

    [Fact]
    public void Deposit_ShouldIncreaseBalance()
    {
        // Arrange
        var account = new Account(Guid.NewGuid(), AccountType.Savings);
        var depositAmount = new Money(100);
        
        // Act
        var transaction = account.Deposit(depositAmount, "Test deposit");
        
        // Assert
        account.Balance.Amount.Should().Be(100);
        transaction.Type.Should().Be(TransactionType.Deposit);
        transaction.Amount.Amount.Should().Be(100);
    }

    [Fact]
    public void Withdraw_WithSufficientFunds_ShouldDecreaseBalance()
    {
        // Arrange
        var account = new Account(Guid.NewGuid(), AccountType.Checking);
        account.Deposit(new Money(200), "Initial deposit");
        
        // Act
        var transaction = account.Withdraw(new Money(50), "Test withdrawal");
        
        // Assert
        account.Balance.Amount.Should().Be(150);
        transaction.Type.Should().Be(TransactionType.Withdrawal);
    }

    [Fact]
    public void Withdraw_WithInsufficientFunds_ShouldThrowException()
    {
        // Arrange
        var account = new Account(Guid.NewGuid(), AccountType.Checking);
        account.Deposit(new Money(50), "Initial deposit");
        
        // Act
        var act = () => account.Withdraw(new Money(100), "Over withdrawal");
        
        // Assert
        act.Should().Throw<InsufficientFundsException>()
            .And.RequestedAmount.Amount.Should().Be(100);
    }

    [Fact]
    public void Transfer_ShouldDebitSourceAndCreditDestination()
    {
        // Arrange
        var sourceAccount = new Account(Guid.NewGuid(), AccountType.Checking);
        var destAccount = new Account(Guid.NewGuid(), AccountType.Savings);
        sourceAccount.Deposit(new Money(500), "Initial");
        
        // Act
        var (outgoing, incoming) = sourceAccount.TransferTo(destAccount, new Money(200));
        
        // Assert
        sourceAccount.Balance.Amount.Should().Be(300);
        destAccount.Balance.Amount.Should().Be(200);
        outgoing.Type.Should().Be(TransactionType.TransferOut);
        incoming.Type.Should().Be(TransactionType.TransferIn);
    }

    [Fact]
    public void Freeze_ShouldPreventTransactions()
    {
        // Arrange
        var account = new Account(Guid.NewGuid(), AccountType.Checking);
        account.Deposit(new Money(100), "Initial");
        account.Freeze();
        
        // Act
        var act = () => account.Withdraw(new Money(50), "Should fail");
        
        // Assert
        act.Should().Throw<AccountNotActiveException>();
    }
}
