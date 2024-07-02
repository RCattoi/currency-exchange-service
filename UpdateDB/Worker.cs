using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using UpdateDB.Infra;

namespace UpdateDB;

public class Worker : BackgroundService
{
  private readonly ILogger<Worker> _logger;

  public Worker(ILogger<Worker> logger)
  {
    _logger = logger;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    RabbitMqHandler rabbit = new();
    rabbit.Connect();
    RetrieveCurrency getCurrency = new();

    while (!stoppingToken.IsCancellationRequested)
    {
      CurrencyListModel currencyToUpdate = await getCurrency.GetCurrencyList();
      rabbit.SendToQueue(currencyToUpdate);
      await Task.Delay(30000, stoppingToken);
    }
  }
}
