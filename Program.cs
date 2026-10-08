using Microsoft.EntityFrameworkCore;

using BankApi.Dtos;
using BankApi.Data;
using BankApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BankDbContext>(options =>
{
    options.UseSqlite("Data Source=bank.db");
});

builder.Services.AddScoped<IAccountService, AccountService>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    message = "bank api is running"
}));

app.MapPost("/accounts", async (
    CreateAccountRequest request,
    IAccountService accountService) =>
{
    if (string.IsNullOrWhiteSpace(request.Owner))
    {
        return Results.BadRequest(new
        {
            error = "owner is required."
        });

    }

    var account = await accountService.CreateAsync(request.Owner);

    var response = new AccountResponse(
        account.Id,
        account.Owner,
        account.Balance,
        account.CreatedAt
    );

    return Results.Created($"/accounts/{account.Id}", response);
});

app.MapGet("/accounts/{id:guid}", async (
    Guid id,
    IAccountService accountService) =>
{
    var account = await accountService.GetByIdAsync(id);

    if (account is null)
    {
        return Results.NotFound(new
        {
            error = "account not found."
        });
    }

    var response = new AccountResponse(
        account.Id,
        account.Owner,
        account.Balance,
        account.CreatedAt
    );

    return Results.Ok(response);
});

app.MapPost("/accounts/{id:guid}/deposits", async (
    Guid id,
    AccountTransactionRequest request,
    IAccountService accountService) =>
{
    if (request.Amount <= 0)
    {
        return Results.BadRequest(new
        {
            error = "Amount must be greater than zero."
        });
    }

    var account = await accountService.DepositAsync(id, request.Amount);

    if (account is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(new AccountResponse(
        account.Id,
        account.Owner,
        account.Balance,
        account.CreatedAt
    ));
});

app.MapPost("/accounts/{id:guid}/withdrawals", async (
    Guid id,
    AccountTransactionRequest request,
    IAccountService accountService) =>
{
    if (request.Amount <= 0)
    {
        return Results.BadRequest(new
        {
            error = "Amount must be greater than zero."
        });
    }

    try
    {
        var account = await accountService.WithdrawAsync(id, request.Amount);

        if (account is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(new AccountResponse(
            account.Id,
            account.Owner,
            account.Balance,
            account.CreatedAt
        ));
    }
    catch (InvalidOperationException exception)
    {
        return Results.BadRequest(new
        {
            error = exception.Message
        });
    }
});

app.Run();
