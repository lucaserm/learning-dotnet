using System.Net;
using System.Net.Http.Json;
using BankApi.Dtos;
using Xunit;

namespace BankApi.Tests;

public class AccountIntegrationTests
{
    [Fact]
    public async Task Deposit_ShouldPersistBalance()
    {
        using var factory = new BankApiFactory();
        using var client = factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync(
            "/accounts",
            new CreateAccountRequest("Lucas")
        );

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var account = await createResponse.Content
            .ReadFromJsonAsync<AccountResponse>();

        Assert.NotNull(account);

        var depositResponse = await client.PostAsJsonAsync(
            $"/accounts/{account.Id}/deposits",
            new AccountTransactionRequest(100m)
        );

        Assert.Equal(HttpStatusCode.OK, depositResponse.StatusCode);

        var savedAccount = await client.GetFromJsonAsync<AccountResponse>(
            $"/accounts/{account.Id}"
        );

        Assert.NotNull(savedAccount);
        Assert.Equal(100m, savedAccount.Balance);
    }

    [Fact]
    public async Task Withdraw_ShouldReturnBadRequest_WhenFundsAreInsufficient()
    {
        using var factory = new BankApiFactory();
        using var client = factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync(
            "/accounts",
            new CreateAccountRequest("Lucas")
        );

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var account = await createResponse.Content
            .ReadFromJsonAsync<AccountResponse>();

        Assert.NotNull(account);

        var withdrawResponse = await client.PostAsJsonAsync(
            $"/accounts/{account.Id}/withdrawals",
            new AccountTransactionRequest(50m)
        );

        Assert.Equal(
            HttpStatusCode.BadRequest,
            withdrawResponse.StatusCode
        );

        var savedAccount = await client.GetFromJsonAsync<AccountResponse>(
            $"/accounts/{account.Id}"
        );

        Assert.NotNull(savedAccount);
        Assert.Equal(0m, savedAccount.Balance);
    }
}