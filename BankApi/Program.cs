using Microsoft.EntityFrameworkCore;

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

builder.Services.AddControllers();

var app = builder.Build();

app.UseMiddleware<BusinessExceptionMiddleware>();

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    message = "bank api is running"
}));

app.MapControllers();

app.Run();

public partial class Program { }