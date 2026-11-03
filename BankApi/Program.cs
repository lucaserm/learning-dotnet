using Microsoft.EntityFrameworkCore;

using BankApi.Dtos;
using BankApi.Data;
using BankApi.Services;
using BankApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration
    .GetConnectionString("BankDatabase")
    ?? throw new InvalidOperationException(
        "Connection string 'BankDatabase' not found."
    );

builder.Services.AddDbContext<BankDbContext>(options =>
{
    options.UseSqlite(connectionString);
});

builder.Services.AddScoped<IAccountService, AccountService>();

var app = builder.Build();

app.UseMiddleware<BusinessExceptionMiddleware>();

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
    return Results.Created($"/accounts/{account.Id}", account.ToResponse());
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

    return Results.Ok(account.ToResponse());
});

app.MapPost("/accounts/{id:guid}/deposits", async (
    Guid id,
    AccountTransactionRequest request,
    IAccountService accountService) =>
{
    var account = await accountService.DepositAsync(id, request.Amount);

    if (account is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(account.ToResponse());
});

app.MapPost("/accounts/{id:guid}/withdrawals", async (
    Guid id,
    AccountTransactionRequest request,
    IAccountService accountService) =>
{
    var account = await accountService.WithdrawAsync(id, request.Amount);

    if (account is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(account.ToResponse());
});

app.MapGet("/accounts", async (
    IAccountService accountService,
    int page = 0,
    int pageSize = 10) =>
{
    var response = await accountService.ListAsync(page, pageSize);

    return Results.Ok(response);
});

app.Run();

public partial class Program { }