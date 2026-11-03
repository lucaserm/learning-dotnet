using BankApi.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddWindowsService();
builder.Services.AddHttpClient("bank", client =>
    client.BaseAddress = new Uri(builder.Configuration["Api:BaseUrl"]!));
builder.Services.AddHostedService<Worker>();

builder.Build().Run();
