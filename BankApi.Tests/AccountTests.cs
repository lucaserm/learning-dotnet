using BankApi.Exceptions;
using BankApi.Models;
using Xunit;

namespace BankApi.Tests;

public class AccountTests
{
    [Fact]
    public void Deposit_ShouldIncreaseBalance()
    {
        var account = new Account("Lucas");

        account.Deposit(100m);

        Assert.Equal(100m, account.Balance);
    }

    [Fact]
    public void Withdraw_ShouldDecreaseBalance()
    {
        var account = new Account("Lucas");
        account.Deposit(100m);

        account.Withdraw(30m);

        Assert.Equal(70m, account.Balance);
    }

    [Fact]
    public void Withdraw_ShouldAllowEntireBalance()
    {
        var account = new Account("Lucas");
        account.Deposit(100m);

        account.Withdraw(100m);

        Assert.Equal(0m, account.Balance);
    }

    [Fact]
    public void Withdraw_ShouldPreserveBalance_WhenFundsAreInsufficient()
    {
        var account = new Account("Lucas");
        account.Deposit(100m);

        Assert.Throws<BusinessRuleException>(
            () => account.Withdraw(101m)
        );

        Assert.Equal(100m, account.Balance);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Deposit_ShouldRejectNonPositiveAmount(int amount)
    {
        var account = new Account("Lucas");
        account.Deposit(100m);

        Assert.Throws<BusinessRuleException>(
            () => account.Deposit(amount)
        );

        Assert.Equal(100m, account.Balance);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Withdraw_ShouldRejectNonPositiveAmount(int amount)
    {
        var account = new Account("Lucas");
        account.Deposit(100m);

        Assert.Throws<BusinessRuleException>(
            () => account.Withdraw(amount)
        );

        Assert.Equal(100m, account.Balance);
    }
}