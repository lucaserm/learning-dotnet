namespace BankApi.Worker;

public class Worker(ILogger<Worker> logger, IHttpClientFactory factory) : BackgroundService
{
  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    var client = factory.CreateClient("bank");
    using var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));

    do
    {
      try
      {
        var response = await client.GetAsync("/health", stoppingToken);
        logger.LogInformation("API health: {statusCode}", (int)response.StatusCode);
      }
      catch (HttpRequestException ex)
      {
        logger.LogWarning("API is down: {message}", ex.Message);
      }
    } while (await timer.WaitForNextTickAsync(stoppingToken));
  }
}
