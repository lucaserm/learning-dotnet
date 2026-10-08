var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new {
    status = "ok",
    message = "bank api is running"
}));

app.Run();
