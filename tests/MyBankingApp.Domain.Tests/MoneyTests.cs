using FluentAssertions;
using MyBankingApp.Domain.ValueObjects;

namespace MyBankingApp.Domain.Tests;

public class MoneyTests
{
    [Fact]
    public void Money_ShouldRoundToTwoDecimalPlaces()
    {
        // Act
        var money = new Money(10.999m);
        
        // Assert
        money.Amount.Should().Be(11.00m);
    }

    [Fact]
    public void Money_ShouldNotAllowNegativeAmount()
    {
        // Act
        var act = () => new Money(-10);
        
        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Money_Addition_ShouldWork()
    {
        // Arrange
        var money1 = new Money(10);
        var money2 = new Money(20);
        
        // Act
        var result = money1 + money2;
        
        // Assert
        result.Amount.Should().Be(30);
    }

    [Fact]
    public void Money_Subtraction_ShouldWork()
    {
        // Arrange
        var money1 = new Money(50);
        var money2 = new Money(20);
        
        // Act
        var result = money1 - money2;
        
        // Assert
        result.Amount.Should().Be(30);
    }

    [Fact]
    public void Money_Comparison_ShouldWork()
    {
        // Arrange
        var moreMoney = new Money(100);
        var lessMoney = new Money(50);
        
        // Assert
        (moreMoney > lessMoney).Should().BeTrue();
        (lessMoney < moreMoney).Should().BeTrue();
    }

    [Fact]
    public void Money_DifferentCurrencies_ShouldThrowOnAddition()
    {
        // Arrange
        var usd = new Money(10, "USD");
        var eur = new Money(10, "EUR");
        
        // Act
        var act = () => usd + eur;
        
        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*different currencies*");
    }
}
