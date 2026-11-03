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

    [Fact]
    public async Task GetById_ShouldReturnAccount_WhenItExists()
    {
        using var factory = new BankApiFactory();
        using var client = factory.CreateClient();

        var created = await CreateAccountAsync(client, "Lucas");

        var response = await client.GetAsync($"/accounts/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var account = await response.Content.ReadFromJsonAsync<AccountResponse>();

        Assert.NotNull(account);
        Assert.Equal(created.Id, account.Id);
        Assert.Equal("Lucas", account.Owner);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenAccountDoesNotExist()
    {
        using var factory = new BankApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"/accounts/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenOwnerIsBlank()
    {
        using var factory = new BankApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/accounts",
            new CreateAccountRequest("   ")
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Deposit_ShouldReturnNotFound_WhenAccountDoesNotExist()
    {
        using var factory = new BankApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            $"/accounts/{Guid.NewGuid()}/deposits",
            new AccountTransactionRequest(10m)
        );

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task List_ShouldPaginateAccounts()
    {
        using var factory = new BankApiFactory();
        using var client = factory.CreateClient();

        foreach (var owner in new[] { "A", "B", "C" })
        {
            await CreateAccountAsync(client, owner);
        }

        var firstPage = await client
            .GetFromJsonAsync<PagedResponse<AccountResponse>>("/accounts?page=0&pageSize=2");
        var secondPage = await client
            .GetFromJsonAsync<PagedResponse<AccountResponse>>("/accounts?page=1&pageSize=2");

        Assert.NotNull(firstPage);
        Assert.NotNull(secondPage);
        Assert.Equal(2, firstPage.Items.Count);
        Assert.Single(secondPage.Items);
        Assert.Equal(3, firstPage.TotalCount);
    }

    [Fact]
    public async Task List_ShouldReturnBadRequest_WhenOffsetOverflows()
    {
        using var factory = new BankApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync(
            $"/accounts?page={int.MaxValue}&pageSize=100"
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private static async Task<AccountResponse> CreateAccountAsync(
        HttpClient client,
        string owner)
    {
        var response = await client.PostAsJsonAsync(
            "/accounts",
            new CreateAccountRequest(owner)
        );

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var account = await response.Content.ReadFromJsonAsync<AccountResponse>();

        Assert.NotNull(account);

        return account;
    }
}