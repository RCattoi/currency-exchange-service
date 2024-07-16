using System.Text;
using DataConnectionLib;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using UpdateDB.Infra;
using UpdateDB.Models;

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
    RabbitConnection rabbit = new();
    rabbit.Connect();
    rabbit.ConfigureQueue(new List<string> { "redis", "sql" });
    if (rabbit._channel == null)
    {
      Console.WriteLine("Canal não configurado");
      return;
    }
    RabbitMqHandler sender = new(rabbit._channel);
    RetrieveCurrency getCurrency = new();

    while (!stoppingToken.IsCancellationRequested)
    {
      CurrencyListModel currencyToUpdate = await getCurrency.GetCurrencyList();
      MessageModel message = new("update", currencyToUpdate);
      sender.SendToQueue(message);
      await Task.Delay(30000, stoppingToken);
    }
  }
}
