using BankApi.Dtos;
using BankApi.Data;
using BankApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BankDbContext>(options =>
{
    options.UseSqlite("Data Source=bank.db");
});

builder.Services.AddScoped<IAccountService, AccountService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<BankDbContext>();

    await db.Database.EnsureCreatedAsync();
}

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

    return Results.Created($"/accounts/{account.Id}", account);
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

    return Results.Ok(account);
});

app.Run();
