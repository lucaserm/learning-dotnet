using BankApi.Dtos;
using BankApi.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IAccountService, AccountService>();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    message = "bank api is running"
}));

app.MapPost("/accounts", (
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

    var account = accountService.Create(request.Owner);

    return Results.Created($"/accounts/{account.Id}", account);
});

app.MapGet("/accounts/{id:guid}", (Guid id, IAccountService accountService) =>
{
    var account = accountService.GetById(id);

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
